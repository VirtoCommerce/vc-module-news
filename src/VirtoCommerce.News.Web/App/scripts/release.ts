import { release } from "@vc-shell/release-config";
import { sync } from "cross-spawn";

release({
  packages: ["."],
  toTag: (version) => `v${version}`,
  bumpVersion: async (_pkgName, _pkgVersion) => {
    const bumpArgs = ["workspace", _pkgName, "version", _pkgVersion, "--deferred"];
    await sync("yarn", bumpArgs);

    const versionApplyArgs = ["version", "apply", "--all"];
    await sync("yarn", versionApplyArgs);
  },
  generateChangelog: async (_pkgName, _pkgVersion, _workspaceName) => {
    const changelogArgs = ["conventional-changelog", "-p", "angular", "-i", "CHANGELOG.md", "-s", "--commit-path", "."];
    await sync("npx", changelogArgs, { cwd: _workspaceName ? `${_workspaceName}` : "." });
  },
});
