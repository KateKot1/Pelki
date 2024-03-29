using System;
using Pelki.Gameplay.Inputs;

namespace Pelki.Gameplay.Puzzles
{
    public interface IPuzzle
    {
        public event Action Solved;
    }
}