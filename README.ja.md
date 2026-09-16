# Jamie Hackathon 2026

物理的な RFID トークンで能力を切り替える 2D スピードランプラットフォーマーです。

Unity、ESP32、MFRC522 RFID リーダーを使い、ハッカソンで一人で2〜3日かけて制作しました。リーダーは Bluetooth 経由でキーボード入力を送るため、専用ハードウェアがなくても通常のキーボードで遊べます。

[English README](README.md)

## ゲームをプレイ

[ブラウザでプレイする](https://play.unity.com/en/games/51c4cc8b-b2fc-4f07-a06d-968e66fd5fc3/polished) — セットアップ不要

## 現在の状況

ハッカソン後も、移動、レベルの見やすさ、能力 HUD、ブラウザ版を調整しています。クラッシュ検出、リセット処理、レースとリーダーボードの細かなケースはまだ作業中です。

## 操作方法

ゲーム画面をクリックして入力を有効にしてください。

| 入力 | 操作 |
| --- | --- |
| A / D | 移動 |
| Space | ジャンプ。早く離すと低くジャンプ |
| 1 | ダッシュを選択 |
| 2 | ウォールジャンプを選択 |
| 3 | 重力反転を選択 |
| 4 | 時間停止を選択 |
| Left Shift | 選択した能力を発動 |
| 壁の近くで Space | 能力2選択中にウォールジャンプ |

RFID トークンをスキャンすると、数字キーと同じように能力が選択されます。

## ゲームの特徴

- 加速、コヨーテタイム、ジャンプ入力のバッファ、可変ジャンプ高度
- ダッシュ、ウォールスライド／ウォールジャンプ、重力反転、時間停止
- 動く障害物、チュートリアル、能力 HUD
- スピードランタイマー、ローカルリーダーボード、ゴーストリプレイ
- キーボードと物理 RFID による能力選択

## RFID を使った理由

テーマが「重力」のゲームジャムで2Dプラットフォーマーを作った経験から、一つの移動ルールを変えるとゲームの感触が大きく変わることに興味を持ちました。その後、Disney Infinity のフィギュアを見て、物理的な物体がゲームと連動する仕組みを小さな形で試すことにしました。

RFID トークン → MFRC522 リーダー → ESP32 → Bluetooth キーボード入力 → Unity

Bluetooth HID を使っているので、Unity 側では普通のキーボード入力として受け取れます。

## RFID コントローラー

ハードウェアは ESP32、SPI 接続の MFRC522 RFID リーダー、RFID タグまたはキーフォブです。Bluetooth デバイス Hackathon Keyboard をペアリングしてから、ゲーム画面をクリックしてトークンをスキャンします。

タグの能力文字列は次の通りです。

    dash000000000000
    wall000000000000
    gravity000000000
    timestop00000000

それぞれキー 1、2、3、4 に対応します。

## プロジェクトを開く

    git clone https://github.com/EyeInDisguise/Jamie-Hackathon-2026.git
    cd Jamie-Hackathon-2026
    git lfs install
    git lfs pull

Unity Hub で Unity 6000.5.6f1 と Web Build Support をインストールし、Add project from disk からプロジェクトを開きます。Assets/Scenes/MainMenu.unity を開いて Play を押してください。画像や音声は Git LFS で管理しています。

ファームウェアは RFIDHackathon ディレクトリの PlatformIO プロジェクトです。

    pio run                 # ビルド
    pio run -t upload       # ESP32 にアップロード
    pio device monitor      # シリアルモニター（115200 baud）

## クレジット

[ESP32 と MFRC522](https://randomnerdtutorials.com/esp32-mfrc522-rfid-reader-arduino/) · [Unity チュートリアル](https://generalistprogrammer.com/tutorials/unity-2d-platformer-complete-tutorial-game-development) · [Tarodev](https://www.youtube.com/@Tarodev) · [音楽: Get Kominami](https://getkominami.com/bgm)

## AI の使用について

開発中、プログラミング、デバッグ、説明、ドキュメント作成の補助として AI を使いました。提案は自分で確認・テストし、プロジェクトに合わせて変更しています。コンセプト、ゲームデザイン、ハードウェア連携、3Dプリント、実装上の判断、テスト、最終的なハッカソン提出は自分が担当し責任を持っています。
