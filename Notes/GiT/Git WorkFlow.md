**Git workflows** are a recipe or recommendation for how to use Git to accomplish work in a consistent and productive manner. They encourage developers and DevOps teams to leverage Git effectively and consistently. Git offers a lot of flexibility in how users manage changes.

**Git tags** are references showing particular points in a Git history. The main function of tagging is to capture a point in a Git history that marks version release1. Git tags are majorly used for marking a particular milestone release of your code. One of the major use of git tag is to mark your release version. You would find all the code base to follow a particular pattern like, v3.5.22.
![[tags.webp]]

There are many Git workflows available, but some of the most popular ones are:

**Centralized Workflow:** This workflow uses a central repository to serve as the single point-of-entry for all changes to the project. Developers clone the repository and make changes locally before pushing them back to the central repository.
Main characteristics: one main central repo
Pros+: simple and easy to understand
Cons-: not scalable, slow

**Feature Branch Workflow:** This workflow is similar to the Centralized Workflow, but it adds a feature branch for each new feature or change. Developers createa new branch off the main branch, make changes locally, and then merge their changes back into the main branch.
Main characteristics: separate branches for features on top of main repo
Pros+: separated, individually managed features
Cons-: not scalable, potential cross-conflicts

**Forking Workflow:** This workflow is designed for open-source projects where anyone can contribute. Instead of having a single central repository, each developer has their own fork of the repository. Developers create feature branches off their own fork, make changes locally, and then submit pull requests to have their changes merged into the main repository.
Main characteristics: everyone has their own repo
Pros+: keeps branches separated, everyone works on their own
Cons-: difficult to keep track on all active branches, collab is harder.

**Gitflow Workflow:** This workflow is designed for larger projects with multiple release cycles. It uses two main branches: master and develop. Developers create feature branches off the develop branch, make changes locally, and then merge their changes back into the develop branch. When it’s time to release a new version of the software, a release branch is created off the develop branch.
Main characteristics: master + develop branch supported by hotfix and feature branches
Pros+: easy to manage releases and collab on features, scalable
Cons-: difficult to manage feature branches and up-to-date with the main branch, slow

![[02 Feature branches.svg]]

Here are some examples of branch naming conventions that developers use in Gitflow:

Feature branches: feature/feature-name
Release branches: release/version-number
Hotfix branches: hotfix/issue-number