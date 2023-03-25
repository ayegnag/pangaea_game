using System;

namespace KOI
{
    public class OnHoverEntityArgs : EventArgs
    {
        public Dog Dog;

        public OnHoverEntityArgs(Dog dog)
        {
            Dog = dog;
        }
    }
}