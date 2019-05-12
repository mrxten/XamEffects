using System;
namespace XamEffects {
    public static class Effects {
        [Obsolete("Not need with usual Linking")]
        public static void Init() {
            TouchEffect.Init();
            Commands.Init();
            EffectsConfig.Init();
        }
    }
}