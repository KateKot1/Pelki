using Pelki.Configs;
using UnityEngine;

namespace Pelki.Gameplay.Inputs
{
    public class InputBySimpleInput : IInput
    {
        private readonly InputConfig inputConfig;

        public float Horizontal => SimpleInput.GetAxis(inputConfig.HorizontalAxisKey);
        public float RawHorizontal => SimpleInput.GetAxisRaw(inputConfig.HorizontalAxisKey);
        public bool IsJump => SimpleInput.GetButton(inputConfig.JumpKey);
        public bool IsRangedAttacking => SimpleInput.GetButton(inputConfig.RangedAttackKey);

        public InputBySimpleInput(InputConfig inputConfig)
        {
            this.inputConfig = inputConfig;
        }
    }
}