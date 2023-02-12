namespace KOI
{
    public class GameSystem 
    {
        public virtual void Init() {}
        protected virtual void Tick(object sender, OnTickArgs eventArgs) {}
        public virtual void Quit() {}
    }
}