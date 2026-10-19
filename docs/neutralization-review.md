# 中性命名與本機驗收紀錄

日期：2026-10-19。以下保留初輪本機驗收紀錄；後續 review 與本機提交授權見文末。遠端尚未改名。

## 範圍與工作方式

- 原 repository：`https://github.com/Larry-kang/TWQR-Transaction-Core`。
- 本機：`/workspace/TWQR-Transaction-Core`，原先無 checkout，從原 repository 完整 clone；未建立替代遠端 repository。
- 起點：`main` / `c27271c0b6649ca1d930202b57f43743fe55ed9e`；開始時無未提交變更。
- 工作分支：`refactor/neutral-qr-payment-poc`；origin fetch/push 保留原網址。
- 未找到適用的 AGENTS.md、GitHub CI、Docker、部署腳本或額外工作流程檔。已讀 `.agent/rules` 兩份規則與全部八份規格；GAS 規則不適用。遠端 branch protection / rulesets 未取得，標為 NOT_RUN，不影響本機修改，遠端操作前需另查。
- 本輪依使用者規格只處理命名、定位、樣本與驗證；原規格的廣泛平台能力皆標為未來草案，不實作、不升級框架或套件。

## 實際 diff 範圍

1. Solution 改為 `QR-Payment-Core.sln`，保留 project GUID 與方案組態。
2. src 四層與測試目錄、csproj、ProjectReference、namespace、using、RootNamespace、AssemblyName 改為 `QrPayment.*`；HTTP 檔與變數同步。
3. EF migration 的 namespace 與模型 CLR 名稱同步；保留 `20251226072135_InitialCreate`、`Transactions`、欄位、索引、migration SQL 結構與列舉數值。
4. SQLite 預設新檔名改為 `qr-payment.db`。這不是檔案搬遷；既有使用者須明確提供舊資料庫 connection string 或自行備份後搬移。
5. 移除目前版本的舊 SQLite 檔，忽略 SQLite、EF 本機工具。歷史仍可還原原檔。
6. README、規格、專案 agent rule 中性化，範例改為合成 POST。刪除 Controller 中過時且帶損壞編碼的 AI 自述註解及重複的同值狀態賦值；未變更處理流程。
7. 新增本驗收紀錄及文字驗證證據；未新增 CI、Docker 或部署能力。

## 去識別盤點

盤點目前所有受追蹤檔案、全部可取得 refs 的 18 筆提交及其文字 blob、歷史 URL 與二進位資料庫。非正式安全稽核，無法保證所有格式的機密均可被辨識；不包含 GitHub issues、其他 fork 或未取得的遠端 refs。

| 類別 | 發現與處理 |
|---|---|
| 舊專案名稱及大小寫變體 | solution、目錄、專案、C#、EF 模型、規則、README、規格、HTTP 與 DB 檔名已中性化。原名稱只保留在本紀錄的來源／歷史說明及原版驗證證據 |
| 品牌／機構 | 舊 README 的 FISC／財金、iPASS／一卡通與公司正式系統描述移除；未發現品牌圖示或圖片。規格中專有協定排除項改為所有支付品牌通用邊界；未將品牌特定協定冒充通用實作 |
| 網址、路徑、識別碼 | 僅發現 localhost、公開 GitHub、Microsoft、Docker、shields、LinkedIn 與 schema 連結；未發現內部網址。舊 badges 曾宣稱 build-passing / Docker-Enabled，現版未沿用。沒有真實支付 API 串接 |
| 設定與憑證 | appsettings 僅 logging / AllowedHosts；launchSettings 為 localhost。歷史文字格式掃描未發現疑似 private key、token 或硬編碼密碼／金鑰；未發現需因有效憑證暫停發布的項目 |
| 資料庫／交易樣本 | 原 `src/TWQR.WebAPI/twqr.db` 有三個表：migration lock 0 筆、migration history 1 筆、Transactions 0 筆。沒有交易資料可宣稱已去識別；整個檔案從目前版本移除。新範例只用 fictional-merchant / synthetic-order / synthetic-key |
| 保留的契約 | `POST /api/Payment`、DTO／JSON 欄位、`TWD`、資料表、欄位、唯一索引、migration ID、狀態數值均保留，避免破壞持久化或 API 相容性；它們不是品牌專有契約 |
| 通用技術名稱 | Microsoft、SQLite、xUnit、Redis 等技術／套件名稱保留；本輪不改第三方套件名稱或 LICENSE |

改名不代表歷史名稱已被清除。舊 README、名稱、提交訊息、舊檔案路徑與 SQLite blob 仍在 Git 歷史中；沒有重寫歷史或 force push。遠端名稱、origin 與本機目錄刻意仍保留原名稱，直到使用者驗收後另行決定。

## 驗證結果

環境原先無 dotnet。取得 .NET SDK **10.0.402**（runtime 10.0.13），專案仍為 net10.0，套件版本不變；EF 本機工具為 10.0.1。SDK、工具與測試資料庫皆放在 repository 外的 `/workspace/scratch`。

| 項目 | 原版 | 改名後 |
|---|---|---|
| restore | PASS | PASS |
| build --no-restore | PASS，0 error／20 warnings | PASS，0 error／20 warnings |
| test --no-build | PASS，1 個空白測試 | PASS，1 個空白測試 |
| 新 SQLite migration | — | PASS，套用原 migration ID |
| 舊 SQLite 相容性 | 原檔作為基準 | PASS，不重複套用 migration |
| EF pending model changes | — | PASS，無模型變更 |

[原版 restore](verification/baseline-restore.txt)、[build](verification/baseline-build.txt)、[test](verification/baseline-test.txt)；[改名後 restore](verification/renamed-restore.txt)、[build](verification/renamed-build.txt)、[test](verification/renamed-test.txt)；[新 DB](verification/ef-fresh.txt)、[舊 DB](verification/ef-existing.txt)、[模型](verification/ef-model.txt)。

API smoke：**PASS（改名前後）**。啟動各版本編譯產物，使用 repository 外的舊 DB 副本與新 migration DB，比較 Swagger 路徑、首次 POST=200／Created、重複 key=409、缺少 key=400，並直接查 SQLite 確認合成金額 100／TWD／Created、僅一筆交易。兩個服務驗證後已停止，測試資料沒有回寫 repository。[smoke 結果](verification/smoke.txt)。smoke 探針最初把 SQLite numeric affinity 的數值預期為字串而失敗；修正探針以數值比較後重跑，沒有修改應用程式來配合探針。

重現：依 README 建立空 DB、啟動 API，再依序送出 HTTP 範例兩次及移除 Idempotency-Key 的請求；預期 200、409、400。另使用 `dotnet-ef migrations has-pending-model-changes --project src/QrPayment.Infrastructure --startup-project src/QrPayment.WebAPI --no-build` 檢查模型。

原版與改名後皆有既有 NU1902／NU1903 警告，涉及 Microsoft.OpenApi 2.3.0、SQLitePCLRaw.lib.e_sqlite3 2.1.11、System.Security.Cryptography.Pkcs 9.0.0、System.Security.Cryptography.Xml 9.0.0；詳細 advisory 見 restore logs。未為本輪改名升級依賴，正式使用前需另輪處理。安全性稽核、完整 QR／付款流程、併發／負載／真實支付測試與部署均為 **NOT_RUN**，未實作能力不能由空白測試推論通過。

最後靜態檢查：**PASS**。50 個預定版本檔案的 Markdown 相對連結、solution／ProjectReference、JSON、RootNamespace／AssemblyName、migration 與 Transaction 的命名以外差異均已檢查；無資料庫、bin／obj、憑證或暫存產物納入交付。舊名稱殘留逐項限定於本紀錄與原始驗證輸出中的本機路徑／原版名稱，保留作為來源證據。CI／Docker 引用檢查：原庫沒有相關設定，無需改寫。`git diff --check` 與使用獨立暫存索引納入新增檔後的 diff 檢查均通過；真實 Git index 未暫存變更。

## 驗收後的具體交付步驟（本輪未執行）

1. 檢查 `git status --short`、所有 rename 與新增檔，再用 `git add -A` 暫存本輪成果；`git diff --cached --check` 與 `git diff --cached --stat` 驗收。特別確認無 DB／bin／obj／工具／憑證。核准後才 `git commit -m "refactor: neutralize QR payment POC naming"`。
2. 核准正式改名後，到**原** repository 的 Settings → General → Repository name 改為 `QR-Payment-Core`。不建立替代 repository、不刪除原庫；保留原 repository 身分、歷史與既有資產，改名前檢查新名稱未被占用及遠端規則。
3. 正式改名後，本機執行 `git remote set-url origin https://github.com/Larry-kang/QR-Payment-Core.git`，再 `git remote -v`、`git ls-remote origin HEAD` 核對。可自行將本機資料夾改名，但這不影響遠端或 Git 歷史。
4. 取得 push／PR 授權後，`git push -u origin refactor/neutral-qr-payment-poc`，依原 repository rules 建立 PR；merge 另需授權，不使用 force push。正式新連結預期為 `https://github.com/Larry-kang/QR-Payment-Core`，目前不宣稱存在。
5. 建議 description：`主被掃支付交易核心｜C#/.NET 個人 POC：通用 QR 支付情境、模擬服務與交易可靠性探索；目前為早期交易記錄骨架。`
6. 建議 topics：`csharp`, `dotnet`, `aspnet-core`, `qr-payment`, `proof-of-concept`, `sqlite`, `ai-assisted`。不使用暗示支付品牌官方實作或 production-ready 的標籤。

## 待後續同步的連結清單（未修改）

- 履歷 PDF／履歷原稿中的專案名稱、說明與 repository URL（實際檔案位置未提供）。
- LinkedIn 個人檔案的 Projects／Featured／Experience 中相關作品連結。
- 104、Cake 的作品集與履歷欄位。
- GitHub 個人 README、其他作品索引或 portfolio 中指向舊 repository 的連結。

這些位置尚未讀取或確認是否有連結；均為後續人工盤點候選。以新正式 URL 同步，避免依賴舊網址轉址；本輪沒有修改任何個人資料或其他 repository。

## 下一輪履歷可用介紹

**主被掃支付交易核心（QR-Payment-Core）**：使用 C#／.NET 10 與 SQLite 建立個人支付交易 POC，探索使用者掃商家 QR 的主掃與商家掃使用者付款 QR 的被掃情境。目前已具備分層骨架、交易記錄 API、基本重複 key 檢查與資料庫唯一索引，並完成建置及基礎 API smoke 驗證；兩種 QR 付款流程、模擬支付服務與完整交易可靠性機制仍待實作。本人負責需求、規格討論及成果驗收，程式主要透過 AI Agent 協作建立；不處理真實金流。

## 後續 review 與本機提交授權

使用者追加「請自行review 提交變更」，授權 review 後在本機工作分支 commit；不包含 push、PR、merge 或正式遠端改名。前述「本輪未執行」及未暫存描述為初輪交付時的快照。

唯讀獨立 reviewer 與本機複查均確認：migration/schema/enum/API 契約保持相容，沒有框架或套件升級。發現一項 P2 文件問題：EF 會以 WebAPI 工作目錄解析相對 SQLite 路徑，README 原先宣稱根目錄不正確。已改為 POSIX shell 先設定根目錄下的絕對路徑，migration 與 API 使用相同連線，並補 PowerShell 變數設定。其餘未發現需修正的具體問題。

修正後重新驗證：restore **PASS**、build **PASS**（0 errors／20 個既有 warnings）、既有測試 **PASS**（1 個空白測試）。以新絕對路徑 DB 套用 migration，再使用 README 的 `dotnet run --project ... --no-launch-profile` 路徑實際啟動；Swagger、POST 200／Created、重送 409、缺 key 400、合成金額與單筆保存均 **PASS**。服務已停止，測試資料庫留在 repository 外；原先相對路徑探查檔已移除。

證據：[restore](verification/review-restore.txt)、[build](verification/review-build.txt)、[test](verification/review-test.txt)、[migration](verification/review-absolute.txt)、[smoke](verification/review-smoke.txt)。既有依賴弱點及未實作範圍不變；未執行遠端或履歷操作。
