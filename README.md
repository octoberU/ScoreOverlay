# audica-mod-starter-template

This template bundles melon-mod and a simple dll-building C# solution + csproj.

## Getting Started

* You'll need Visual Studio community 2017 setup to build.
* MelonLoader must be installed in Audica's directory - see https://melonwiki.xyz/#/README
    * You should have a `MelonLoader\Managed` folder and `version.dll` in your Audica root before proceeding.

### Automated setup (recommended!)
Use `get-started.ps1` (right click => Run With PowerShell) to configure the name / author / version of the mod, copy needed melon loader dll references and add a post-build step to automatically copy the last built debug or release dll to the Audica mods directory.

NOTE: I have not tested the audica directory auto-location for non-steam versions of Audica but it should work! If not, the script should prompt for manual input but please let me know if there are any problems.

### Manual setup

Skip this if the script above completed successfully.

* You'll need to fetch a bunch of references from your Audica installation. This has to happen if melon loader or audica updates (be sure to rebuild these dlls first!). To copy them quickly, invoke `copy-references.ps1` with the path to your audica folder and the project path (`AudicaMod`, relative or absolute). Alternatively, you can painstakingly go fetch these yourself (copy to the /AudicaMod/lib folder):
    ```
    "Audica\MelonLoader\MelonLoader.ModHandler.dll",
    "Audica\MelonLoader\Managed\Assembly-CSharp.dll", 
    "Audica\MelonLoader\Managed\Assembly-CSharp-firstpass.dll", 
    "Audica\MelonLoader\Managed\Il2CppMono.Security.dll", 
    "Audica\MelonLoader\Managed\Il2Cppmscorlib.dll", 
    "Audica\MelonLoader\Managed\Il2CppSystem.Configuration.dll", 
    "Audica\MelonLoader\Managed\Il2CppSystem.Core.dll", 
    "Audica\MelonLoader\Managed\Il2CppSystem.dll", 
    "Audica\MelonLoader\Managed\Il2CppSystem.Runtime.Serialization.dll", 
    "Audica\MelonLoader\Managed\Il2CppSystem.Xml.dll", 
    "Audica\MelonLoader\Managed\Oculus.VR.dll", 
    "Audica\MelonLoader\Managed\Sanford.Multimedia.Midi.dll", 
    "Audica\MelonLoader\Managed\SonyNP.dll", 
    "Audica\MelonLoader\Managed\SteamVR.dll", 
    "Audica\MelonLoader\Managed\TwitchChatter.dll", 
    "Audica\MelonLoader\Managed\UnhollowerBaseLib.dll", 
    "Audica\MelonLoader\Managed\UnhollowerRuntimeLib.dll", 
    "Audica\MelonLoader\Managed\Unity.TextMeshPro.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.AIModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.AnimationModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.AudioModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.CoreModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.IMGUIModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.ParticleSystemModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.Physics2DModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.PhysicsModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.TerrainModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.TextCoreModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.TextRenderingModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.UI.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.UIModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.UnityWebRequestModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.UnityWebRequestWWWModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.VehiclesModule.dll", 
    "Audica\MelonLoader\Managed\UnityEngine.XRModule.dll"
    ```
* Set the Assembly name of your mod in the AudicaModStarter project preferences to name your dll correctly
* Set the name, author and version in Main.cs
* (optional, but handy!) add a post-build step: `copy "$(TargetPath)" "Path\To\Audica\Mods\$(TargetFileName)` (this means you don't have to copy the dll every time you run a build!)

## What now?

Open the main `AudicaModding.sln` and browse to `Main.cs`. You'll see it's setup to log out when you press T on the keyboard. 
Run a build and you should see a new dll file matching the name provided in the get-started script appear in both the local `bin/[Release | Debug]/` and `Audica/Mods` folder.

With MelonMod installed, add `--melonloader.console` to audica's launch options in steam or however you're loading Audica.exe and a debug console should spawn along with Audica. If everything's working, pressing T should log out a message!

## Next Steps

Get involved in the Audica Modding Discord, we're lovely and we can point you in the right direction! https://discord.gg/cakQUt5
For MelonMod specific discussion, advice and updates join the MelonMod discord at https://discord.gg/M6VDhjv

## Updating MelonLoader 

If a new version of MelonLoader or Audica is released, re-run melon loader in the audica root to regenerate references and mod loader. Then just run `copy-references.ps1` in the root with the absolute or relative path to the audica folder and your mod folder (probably `AudicaMod`).

## Other

Pull requests welcome! Or come bug me on the audica modding discord and I'll help where I can.
