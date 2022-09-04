using Interactables.Tools;

namespace Furniture
{
    public sealed class Furniture : Toolable
    {
        protected override void OnToolUse(Tool tool) => OnCarry();

        public void OnCarry()
        {
            
        }

        public void OnPlace()
        {
            
        }
    }
}