using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelKnight.Enums
{
    public enum LiftStatus
    {
        Stand,
        Moving,
        CarryingCharacter,
        WaitingForCharacterToGoIn,
        WaitingForCharacterToGoOut,
    }
}