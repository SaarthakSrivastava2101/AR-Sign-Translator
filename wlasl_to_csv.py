import os
import cv2
import numpy as np
import pandas as pd
from tqdm import tqdm
import random
import mediapipe as mp

WLASL_ROOT = r"D:\WLASL\SL"
OUT_TRAIN = r"D:\ARSignTranslator\data\train.csv"
OUT_TEST = r"D:\ARSignTranslator\data\test.csv"

FRAMES_PER_SAMPLE = 30
LANDMARKS = 21
VALUES_PER_LANDMARK = 3
FEATURES = FRAMES_PER_SAMPLE * LANDMARKS * VALUES_PER_LANDMARK

MAX_WORDS = 60
MAX_VIDEOS_PER_WORD = 60
TEST_SPLIT = 0.2

random.seed(42)

mp_hands = mp.solutions.hands


def extract_frames(video_path, target_frames=30):
    cap = cv2.VideoCapture(video_path)
    total = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))

    if total <= 0:
        cap.release()
        return []

    idxs = np.linspace(0, total - 1, target_frames).astype(int)
    idx_set = set(idxs.tolist())

    frames = []
    i = 0

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        if i in idx_set:
            frames.append(frame)

        i += 1

    cap.release()
    return frames


def landmarks_from_frame(hands, frame):
    img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    res = hands.process(img)

    if not res.multi_hand_landmarks:
        return None

    hand = res.multi_hand_landmarks[0]
    vals = []

    for lm in hand.landmark:
        vals.extend([lm.x, lm.y, lm.z])

    return vals


def video_to_feature_vector(video_path):
    frames = extract_frames(video_path, FRAMES_PER_SAMPLE)
    if len(frames) != FRAMES_PER_SAMPLE:
        return None

    seq = []
    missing = 0

    with mp_hands.Hands(
        static_image_mode=False,
        max_num_hands=1,
        model_complexity=1,
        min_detection_confidence=0.3,
        min_tracking_confidence=0.3
    ) as hands:
        for fr in frames:
            lm = landmarks_from_frame(hands, fr)
            if lm is None:
                missing += 1
                lm = [0.0] * (LANDMARKS * VALUES_PER_LANDMARK)
            seq.extend(lm)

    if len(seq) != FEATURES:
        return None

    return seq


def build_dataset():
    preferred = ["yes", "no", "thank you", "hello", "please", "love", "sorry", "help", "stop", "go"]
    words = [w for w in preferred if os.path.isdir(os.path.join(WLASL_ROOT, w))]
    words = words[:MAX_WORDS]

    all_rows = []

    for word in words:
        word_dir = os.path.join(WLASL_ROOT, word)
        vids = [f for f in os.listdir(word_dir) if f.lower().endswith(".mp4")]

        random.shuffle(vids)
        vids = vids[:MAX_VIDEOS_PER_WORD]

        for v in tqdm(vids, desc=f"Processing '{word}'"):
            path = os.path.join(word_dir, v)
            feat = video_to_feature_vector(path)
            if feat is None:
                continue
            all_rows.append([word] + feat)

    if len(all_rows) == 0:
        raise Exception("No samples extracted. Check WLASL_ROOT path and folders.")

    df = pd.DataFrame(all_rows)
    header = ["label"] + [f"f{i}" for i in range(1, FEATURES + 1)]
    df.columns = header
    return df


def split_and_save(df):
    df = df.sample(frac=1, random_state=42).reset_index(drop=True)

    n_test = int(len(df) * TEST_SPLIT)
    if n_test < 1:
        n_test = 1
    if n_test >= len(df):
        n_test = len(df) - 1

    df_test = df.iloc[:n_test]
    df_train = df.iloc[n_test:]

    os.makedirs(os.path.dirname(OUT_TRAIN), exist_ok=True)

    df_train.to_csv(OUT_TRAIN, index=False)
    df_test.to_csv(OUT_TEST, index=False)

    print("\n✅ Done!")
    print("Train:", df_train.shape)
    print("Test :", df_test.shape)
    print("Saved:", OUT_TRAIN)
    print("Saved:", OUT_TEST)


if __name__ == "__main__":
    df = build_dataset()
    split_and_save(df)
