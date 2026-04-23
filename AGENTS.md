# Cursor Agent Entry Point

Read these files in order before acting:

1. `.github/sdlc-config.json`
2. `.github/copilot-instructions.md`
3. `.cursor/rules/00-sdlc-core.mdc`

For specialist tasks, apply the matching Cursor agent file from `.cursor/agents/`.
For reusable workflows, apply the matching Cursor skill from `.cursor/skills/<skill-name>/SKILL.md`.

Canonical source policy:

- Treat `.github/agents/`, `.github/skills/`, and `.github/instructions/` as source-of-truth content.
- Use `.cursor` content as Cursor-native assets tailored for this repository.
- Do not modify `.github` SDLC assets unless explicitly requested.
