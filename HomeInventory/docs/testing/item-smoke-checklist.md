# Item Workflow Smoke Checklist

Cíl: rychle ověřit, že základní práce s itemy funguje po změnách.

## Preconditions
- API běží
- DB je dostupná
- Uživatel je přihlášen
- Existuje alespoň 1 household a 1 location

---
## 1) Create item
**Given** jsem v konkrétní lokaci
**When** přidám nový item s názvem `Smoke_Item_001`
**Then**
- item se zobrazí v seznamu
- nevznikne duplicitní item při běžném použití
- aplikace nehodí chybu

## 2) Edit item name
**Given** existuje `Smoke_Item_001`
**When** změním název na `Smoke_Item_001_EDIT`
**Then**
- nový název je vidět ihned
- po reloadu view zůstane změna zachována
## 3) Edit description + placement note
**Given** existuje `Smoke_Item_001_EDIT`
**When** nastavím:
- Description: `smoke description`
- Placement note: `smoke shelf A`
**Then**
- hodnoty se uloží bez chyby
- po reloadu view jsou stále stejné

## 4) Persistence check
**When** zavřu a znovu otevřu aplikaci (nebo reloadnu lokaci)
**Then**
- item i jeho změny jsou stále v datech

## 5) Error handling check (API unavailable)
**When** dočasně znepřístupním API a zkusím upravit item
**Then**
- zobrazí se lokalizovaná, srozumitelná chybová hláška
- aplikace nespadne
- po návratu API jde pokračovat normálně

## 6) Cleanup (volitelné)
- smazat testovací item `Smoke_Item_001_EDIT` nebo ho přejmenovat na interní test data

---

## Pass/Fail
- **PASS**: všechny kroky výše prošly bez pádu a s očekávaným výsledkem
- **FAIL**: jakákoli odchylka, pád, nekonzistence dat, chybějící error message

## Report template (do issue/PR komentáře)
- Date:
- Branch/PR:
- Tester:
- Result: PASS / FAIL
- Notes: