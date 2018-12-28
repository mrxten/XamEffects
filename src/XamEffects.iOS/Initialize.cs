using XamEffects.iOS.GestureCollectors;

namespace XamEffects.iOS {
    public static class Effects {
        public static void Init() {
            CommandsPlatform.Init();
            TouchEffectPlatform.Init();
            LongTapGestureCollector.Init();
            TapGestureCollector.Init();
        }
    }
}