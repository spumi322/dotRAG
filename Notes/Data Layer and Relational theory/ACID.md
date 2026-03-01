# ACID Properties

Guarantees that database transactions are processed reliably.

---

## A — Atomicity

> "All or nothing."

```
┌─────────────────────────────────────┐
│         TRANSFER £500               │
│                                     │
│  Step 1: Debit  Account A  ✅       │
│  Step 2: Credit Account B  ❌ FAIL  │
│                                     │
│  ──────────────────────────────     │
│  Result: ROLLBACK — both undone     │
│  Account A: unchanged ✅            │
└─────────────────────────────────────┘
```

Either ALL steps commit, or NONE do. A failure mid-transaction leaves the database as if nothing happened.

---

## C — Consistency

> "Valid state in, valid state out."

```
┌──────────────────────────────────────┐
│  RULE: Balance must never go below 0 │
│                                      │
│  Before TX:  Account A = £200  ✅    │
│  Action:     Withdraw £500           │
│  After TX:   Account A = -£300  ❌   │
│                                      │
│  ──────────────────────────────      │
│  Result: TX REJECTED — rule violated │
└──────────────────────────────────────┘
```

Every transaction must bring the DB from one **valid** state to another. Integrity constraints are never broken.

---

## I — Isolation

> "Concurrent transactions don't see each other's work."

```
┌────────────────────────────────────────────┐
│  Time ──────────────────────────────────►  │
│                                            │
│  TX 1: READ balance = £500                 │
│  TX 1: WRITE balance = £300                │
│                                            │
│  TX 2:      READ balance = ?               │
│             ├─ Sees £500 (pre-TX1)  ✅     │
│             └─ Sees £300 (mid-TX1)  ❌     │
│                                            │
│  Isolation ensures TX2 sees a consistent   │
│  snapshot — not TX1's intermediate state   │
└────────────────────────────────────────────┘
```

Transactions execute as if they were sequential, even when running concurrently.

---

## D — Durability

> "Committed = permanent."

```
┌─────────────────────────────────────────┐
│  TX commits ──► Written to disk         │
│                                         │
│       💾  ──────────────────────►  ✅   │
│                                         │
│  Server crashes 1ms after commit?       │
│  ──────────────────────────────────     │
│  Data is still there after restart. ✅  │
└─────────────────────────────────────────┘
```

Once committed, the transaction survives any failure — power loss, crash, restart.

---

## Quick Reference

```
A ── All or nothing          → ROLLBACK on failure
C ── Rules always enforced   → Constraints never violated  
I ── Transactions isolated   → No dirty reads mid-flight
D ── Committed = permanent   → Survives crashes
```

[[Transactions]] [[SQL]]