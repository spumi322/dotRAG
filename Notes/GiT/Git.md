### _Git — distributed version control system._

Git tracks changes to source code, enabling collaboration, branching, and history management. Every developer has a full copy of the repository.

### _Essential commands_

```bash
git init                    # create new repo
git clone <url>             # copy remote repo locally
git status                  # show working tree status
git add .                   # stage all changes
git commit -m "message"     # commit staged changes
git push origin main        # push to remote
git pull origin main        # fetch + merge from remote
git fetch                   # download remote changes without merging
```

### _Branching & merging_

```bash
git branch feature/login    # create branch
git checkout feature/login  # switch to branch (or: git switch feature/login)
git checkout -b feature/x   # create + switch in one command
git merge feature/login     # merge branch into current branch
git branch -d feature/login # delete branch after merge
```

### _Merge vs Rebase_

- **Merge:** Creates a merge commit. Preserves full branch history. Safe for shared branches.
- **Rebase:** Replays commits on top of another branch. Creates linear history. ⚠️ Never rebase commits already pushed to a shared branch — it rewrites history.

```bash
# Rebase: replay your feature commits on top of latest main
git checkout feature/x
git rebase main
# Then fast-forward merge
git checkout main
git merge feature/x
```

### _Resolving conflicts_

When Git can't auto-merge, it marks conflicts in the file:

```
<<<<<<< HEAD
your changes
=======
their changes
>>>>>>> feature/x
```

Edit the file to resolve, then `git add` and `git commit`.

### _Undoing changes_

```bash
git restore <file>          # discard working directory changes (unstaged)
git restore --staged <file> # unstage a file
git revert <commit>         # create new commit that undoes a past commit (safe)
git reset --soft HEAD~1     # undo last commit, keep changes staged
git reset --hard HEAD~1     # ⚠️ undo last commit, DISCARD changes
```

**`revert` vs `reset`:** `revert` is safe for shared branches (adds a new commit). `reset` rewrites history — only use on local/unpushed commits.

### _Git workflows_

- **Feature Branch:** One branch per feature, merge via pull request. Most common.
- **Git Flow:** `main` + `develop` + feature/release/hotfix branches. Heavier, suited for versioned releases.
- **Trunk-Based:** Short-lived branches, frequent merges to main. Suited for CI/CD.

### _.gitignore_

Specifies files Git should not track. Checked in at repo root.

```
bin/
obj/
*.user
appsettings.Development.json
.vs/
```

### _Common interview questions_

- "Merge vs rebase?" → Merge preserves history, rebase linearizes. Never rebase shared branches.
- "How to undo a pushed commit?" → `git revert` (safe) — never `reset --hard` on shared branches.
- "`git pull` vs `git fetch`?" → `fetch` downloads changes, `pull` = `fetch` + `merge`.
- "What's a pull request?" → A request to merge your branch, enabling code review before merge.