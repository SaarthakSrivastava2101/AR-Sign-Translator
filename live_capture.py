import cv2
import mediapipe as mp
import numpy as np
import pandas as pd
import os
import time
import sys

FRAMES_PER_SAMPLE = 30
LANDMARKS = 21
VALUES_PER_LANDMARK = 3
FEATURES = FRAMES_PER_SAMPLE * LANDMARKS * VALUES_PER_LANDMARK

mp_hands = mp.solutions.hands


def extract_landmarks_from_frame(hands, frame):
    img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    res = hands.process(img)

    if not res.multi_hand_landmarks:
        return None

    hand = res.multi_hand_landmarks[0]
    vals = []
    for lm in hand.landmark:
        vals.extend([lm.x, lm.y, lm.z])
    return vals


def main():
    if len(sys.argv) < 2:
        print("❌ Usage: python live_capture.py <label>")
        return

    label = sys.argv[1].lower()

    BASE_DIR = r"D:\ARSignTranslator\data\live"
    os.makedirs(BASE_DIR, exist_ok=True)

    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("❌ Camera not found")
        return

    print("⏺ Press 'S' to record gesture | 'Q' to quit")

    with mp_hands.Hands(
        static_image_mode=False,
        max_num_hands=1,
        model_complexity=1,
        min_detection_confidence=0.3,
        min_tracking_confidence=0.3
    ) as hands:

        while True:
            ret, frame = cap.read()
            if not ret:
                break

            cv2.imshow("Live Capture", frame)
            key = cv2.waitKey(1) & 0xFF

            if key == ord('q'):
                break

            if key == ord('s'):
                print("⏺ Recording...")
                frames = []

                while len(frames) < FRAMES_PER_SAMPLE:
                    ret, frame = cap.read()
                    if not ret:
                        continue
                    frames.append(frame)
                    cv2.imshow("Live Capture", frame)
                    cv2.waitKey(1)

                seq = []

                for fr in frames:
                    lm = extract_landmarks_from_frame(hands, fr)
                    if lm is None:
                        lm = [0.0] * (LANDMARKS * VALUES_PER_LANDMARK)
                    seq.extend(lm)

                if len(seq) != FEATURES:
                    print("❌ Feature size mismatch")
                    continue

                existing = [f for f in os.listdir(BASE_DIR) if f.startswith(label)]
                file_index = len(existing) + 1

                out_path = os.path.join(BASE_DIR, f"{label}_{file_index}.csv")

                df = pd.DataFrame([[label] + seq])
                df.to_csv(out_path, index=False, header=False)

                print(f"✅ Saved sample to: {out_path}")
                time.sleep(0.5)

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
