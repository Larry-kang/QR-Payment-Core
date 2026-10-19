# 02. Domain & Bounded Contexts

> 規劃草案：本文件描述未來探索方向，不代表已實作或通過驗證；現況以 [README](../../README.md) 與 [本輪驗證](../neutralization-review.md) 為準。本輪僅調整命名與定位，不實作這些能力。

## 1. Bounded Contexts

```text
Payment Platform
├─ Identity
├─ Customer
├─ Merchant
├─ Payment
├─ Wallet
├─ Ledger
├─ QR Payment
├─ Card Payment
├─ Funding
├─ Settlement
├─ Loyalty
├─ Bill Payment
├─ Reconciliation
├─ Notification
└─ External Simulators
```

## 2. Core vs Supporting Domains

### Core Domains

- Payment
- Ledger
- Wallet
- Settlement
- Reconciliation

### Supporting Domains

- Merchant
- Funding
- Loyalty
- Bill Payment
- QR
- Notification

### Generic / Simulated

- Bank
- Card Processor
- Blockchain
- Biller
- Notification Provider

## 3. Ownership Rules

### Payment

Owns:

- PaymentOrder
- PaymentAttempt
- PaymentStatus
- PaymentMethod
- Refund orchestration

Does not own:

- Wallet accounting
- Merchant settlement accounting
- Loyalty balance

### Ledger

Owns：

- LedgerAccount
- LedgerTransaction
- LedgerEntry

Rules:

- Debit total = Credit total
- Ledger entry append-only
- Business correction uses compensating entry, not update/delete

### Wallet

Owns：

- Wallet
- Available balance projection
- Funding / debit coordination

Wallet balance is a projection / operational value; Ledger remains accounting source of truth.

### Settlement

Owns：

- MerchantReceivable aggregation
- SettlementBatch
- SettlementItem
- Bank payout state

### Loyalty

Owns：

- LoyaltyAccount
- PointTransaction
- Earn / Redeem / Expire

Points must not share the money ledger.

## 4. Integration Style

Default:

- In-process command for same transaction boundary
- Domain event for internal state change
- Outbox event for async cross-module integration

Examples:

```text
PaymentSucceeded
  ├─ Ledger posting
  ├─ Loyalty earn
  ├─ Merchant receivable
  └─ Notification
```

Critical money posting should not depend solely on best-effort message delivery.

## 5. V1 Module Direction

Suggested solution structure:

```text
src/
├─ QrPayment.Api
├─ QrPayment.Modules.Payments
├─ QrPayment.Modules.Wallets
├─ QrPayment.Modules.Ledger
├─ QrPayment.Modules.Merchants
├─ QrPayment.Modules.Settlement
├─ QrPayment.Modules.Funding
├─ QrPayment.Modules.Loyalty
├─ QrPayment.Modules.BillPayments
├─ QrPayment.Infrastructure
└─ QrPayment.Simulators
```

V1 may migrate incrementally from the current Clean Architecture skeleton instead of performing a one-shot rewrite.
