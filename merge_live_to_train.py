import os
import pandas as pd

TRAIN_PATH = r"D:\ARSignTranslator\data\train.csv"
LIVE_DIR = r"D:\ARSignTranslator\data\live"

df_train = pd.read_csv(TRAIN_PATH, header=0)

rows = []

for file in os.listdir(LIVE_DIR):
    if not file.endswith(".csv"):
        continue

    path = os.path.join(LIVE_DIR, file)
    df = pd.read_csv(path, header=None)

    if df.shape[1] != df_train.shape[1]:
        print(f"❌ Skipping {file} (feature mismatch)")
        continue

    rows.append(df)

if not rows:
    print("❌ No live samples found")
    exit()

df_live = pd.concat(rows, ignore_index=True)

df_final = pd.concat([df_train, df_live], ignore_index=True)

df_final.to_csv(TRAIN_PATH, index=False)

print("✅ Live samples merged into train.csv")
print("New train size:", df_final.shape)
