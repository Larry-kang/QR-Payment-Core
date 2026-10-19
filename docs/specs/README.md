# Payment Platform Specifications

> 規劃草案：本文件描述未來探索方向，不代表已實作或通過驗證；現況以 [README](../../README.md) 與 [本輪驗證](../neutralization-review.md) 為準。本輪僅調整命名與定位，不實作這些能力。

本目錄定義 QR-Payment-Core 主被掃支付交易 POC 的未來探索草案。

> 狀態：Draft v0.1  
> 原則：先定義 Domain、交易一致性與系統邊界，再開始擴充實作。  
> 本次文件不代表已完成功能；README 或程式碼若與本 SPEC 衝突，後續實作應以經確認後的 SPEC 為準。

## 目標

建立一套後端優先的支付平台示範系統，展示：

- Consumer Wallet 與 Payment Instrument
- QR 主掃（使用者掃商家 QR）/ 被掃（商家掃使用者付款 QR）
- Merchant POS 收單
- Wallet / Card Payment
- Bank Funding
- Crypto Funding（V1.5）
- Merchant Clearing / Settlement
- Loyalty / Points
- Bill Payment
- Idempotency / Concurrency / State Machine
- Async Processing / Outbox / Reconciliation
- External Service Simulation

## 規格索引

1. [Product Scope & System Context](./01-product-scope-system-context.md)
2. [Domain & Bounded Contexts](./02-domain-bounded-contexts.md)
3. [Payment Domain](./03-payment-domain.md)
4. [Wallet & Ledger](./04-wallet-ledger.md)
5. [Reliability / Async / Idempotency](./05-reliability-async-idempotency.md)
6. [Settlement & Reconciliation](./06-settlement-reconciliation.md)
7. [Frontend & UX Scope](./07-frontend-ux.md)
8. [Delivery Roadmap](./08-delivery-roadmap.md)

## 核心設計原則

1. **Money correctness > latency > convenience**
2. Redis、Message Broker、External Provider 都可能失敗，但 Ledger 必須可驗證。
3. Payment 不可因 Retry 重複執行。
4. 外部結果不確定時使用 `Unknown`，不可強制視為成功或失敗。
5. Wallet balance 不作為唯一帳務 Source of Truth；Ledger 才是最終帳務依據。
6. External Service 全部可由 Simulator 取代，以展示整體交易生命週期。
7. V1 採 Modular Monolith，先確保 Domain 與 Transaction Boundary 清楚，不為展示而過早拆 Microservices。
