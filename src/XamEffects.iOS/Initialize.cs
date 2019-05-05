using XamEffects.iOS.GestureCollectors;
using XamEffects.iOS.Renderers;

namespace XamEffects.iOS {
    public static class Effects {
        public static void Init() {
            CommandsPlatform.Init();
            TouchEffectPlatform.Init();
            BorderViewRenderer.Link();
        }
    }
}