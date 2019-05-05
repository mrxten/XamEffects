using System;
namespace XamEffects {
    public static class Effects {
        [Obsolete("Not needed with usual Linking")]
        public static void Init() {
            TouchEffect.Init();
            Commands.Init();
            EffectsConfig.Init();
        }
    }
}