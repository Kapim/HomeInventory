# Location Workflow Smoke Checklist

Cíl: rychle ověřit, že základní práce s lokacemi (create/edit/delete) funguje po změnách.
Doba: cca 8–12 minut.

## Preconditions
- API běží
- DB je dostupná
- Uživatel je přihlášen
- Existuje alespoň 1 household
- V householdu je alespoň jedna kořenová lokace

---

## 1) Create root location
**Given** jsem v aktivním householdu  
**When** vytvořím novou root lokaci `Smoke_Root_001`  
**Then**
- lokace se objeví ve stromu mezi root uzly
- je vybratelná
- bez pádu UI

## 2) Create child location
**Given** mám lokaci `Smoke_Root_001`  
**When** vytvořím pod ní child lokaci `Smoke_Child_001`  
**Then**
- child je vidět pod parent uzlem
- parent se správně expanduje
- child má správný parent

## 3) Rename location
**Given** existuje `Smoke_Child_001`  
**When** přejmenuji ji na `Smoke_Child_001_EDIT`  
**Then**
- nový název je vidět ve stromu
- po reloadu householdu změna zůstává

## 4) Validation check
**Given** edituji název lokace  
**When** zadám prázdný/whitespace název  
**Then**
- uložit se neprovede
- uživatel dostane srozumitelnou validaci/chybu
- původní validní stav zůstane konzistentní

## 5) Delete leaf location
**Given** existuje leaf `Smoke_Child_001_EDIT` bez dětí  
**When** smažu ji  
**Then**
- lokace zmizí ze stromu
- parent zůstane konzistentní
- bez pádu UI

## 6) Delete parent location (behavior rule)
**Given** existuje parent `Smoke_Root_001`  
**When** pokusím se parent smazat  
**Then**
- chování odpovídá domluvenému pravidlu (blokace / kaskádové smazání / přesun)
- uživatel dostane jasnou informaci o výsledku

## 7) Persistence check
**When** reloadnu household (nebo restartuji aplikaci)  
**Then**
- strom odpovídá posledním úspěšně uloženým změnám

## 8) Error handling check (API unavailable)
**When** dočasně znepřístupním API a zkusím create/rename/delete lokaci  
**Then**
- zobrazí se srozumitelná chybová hláška
- UI nezůstane v nekonzistentním stavu
- po obnovení API lze pokračovat bez restartu aplikace

---

## Pass/Fail
- **PASS**: všechny kroky prošly dle očekávání
- **FAIL**: pád aplikace, nekonzistence stromu, neuložené změny bez hlášky, špatné chování delete pravidel

## Report template (do issue/PR komentáře)
- Date:
- Branch/PR:
- Tester:
- Result: PASS / FAIL
- Notes:
