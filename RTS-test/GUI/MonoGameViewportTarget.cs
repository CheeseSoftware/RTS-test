using SharpCEGui.Base;

namespace RTS_test.GUI
{
    public class MonoGameViewportTarget : MonoGameRenderTarget
    {
        public MonoGameViewportTarget(MonoGameRenderer owner)
            : base(owner)
        {
            // initialise renderer size
            var vp = owner.GetDevice().Viewport;

            var area = new Rectf(new Vector2f(vp.X, vp.Y),
                                 new Sizef(vp.Width, vp.Height));

            SetArea(area);
        }

        public MonoGameViewportTarget(MonoGameRenderer owner, Rectf area)
            : base(owner)
        {
            SetArea(area);
        }

        public override bool IsImageryCache()
        {
            return false;
        }
    }
}