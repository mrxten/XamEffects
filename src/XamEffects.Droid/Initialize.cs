using XamEffects.Droid.Collectors;

namespace XamEffects.Droid {
    public static class Effects {
        public static void Init() {
            TouchEffectPlatform.Init();
            CommandsPlatform.Init();
            ViewOverlayCollector.Init();
        }
    }
}