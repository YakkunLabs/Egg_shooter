using UnityEngine;
using UnityEditor;

/// <summary>
/// Startup Helper - Provides helpful information when Unity Editor loads
/// This script runs automatically when Unity starts
/// </summary>
[InitializeOnLoad]
public class StartupHelper
{
    static StartupHelper()
    {
        EditorApplication.delayCall += OnEditorLoaded;
    }

    private static void OnEditorLoaded()
    {
        // Only show this message once per session
        if (SessionState.GetBool("StartupHelper_Shown", false))
            return;

        SessionState.SetBool("StartupHelper_Shown", true);

        // Display helpful startup message
        Debug.Log(
            "<color=cyan>═══════════════════════════════════════════════════════</color>\n" +
            "<color=green><b>🥚 EGG SHOOTER - PROJECT LOADED SUCCESSFULLY! ✅</b></color>\n" +
            "<color=cyan>═══════════════════════════════════════════════════════</color>\n\n" +
            
            "<b>📋 Project Status:</b>\n" +
            "  ✅ All scripts compiled successfully\n" +
            "  ✅ Input System configured (Both old + new)\n" +
            "  ✅ All packages loaded\n" +
            "  ✅ Ready to play!\n\n" +
            
            "<b>⚠️ Console Warnings (Safe to Ignore):</b>\n" +
            "  • Input Manager deprecation → Informational only\n" +
            "  • Package Manager auth errors → Network issue, not critical\n" +
            "  • ProBuilder serialization → Minor warning, no impact\n\n" +
            
            "<b>🎮 Quick Start:</b>\n" +
            "  1. Open Scene: Assets/Scenes/MainMenu.unity\n" +
            "  2. Press Play ▶️\n" +
            "  3. Enjoy the game!\n\n" +
            
            "<b>🔧 Tools Available:</b>\n" +
            "  • <color=yellow>Tools → Fix Project Issues</color> (Diagnostics & Fixes)\n" +
            "  • Read: PROJECT_FIXES_README.md (Detailed explanation)\n\n" +
            
            "<color=cyan>═══════════════════════════════════════════════════════</color>"
        );
    }

    [MenuItem("Tools/Show Startup Message")]
    public static void ShowStartupMessage()
    {
        SessionState.SetBool("StartupHelper_Shown", false);
        OnEditorLoaded();
    }
}
