using PixelKnight.Enums;

namespace PixelKnight.Models
{
    public class AlliancePermissions
    {
        public bool canApprove;
        public bool canKick;
        public bool canView;
        public bool canDeployCrew;
        public bool canMoveRoom;
        public bool canRemoveRoom;
        public bool canDiscardAmmo;
        public bool canDeployItem;
        public bool canBuyAmmo;
        public bool canEditAI;
        public bool canBuildRoom;
        public bool canPaint;
        public bool canSpeedUpAmmo;
        public bool canSpeedUpBuild;
        public bool canUpgradeShip;
        public bool canResearch;
        public bool canEditAlliance;

        public void SetAllianceUserPermissions(AllianceMembership allianceMembership)
        {
            int num = (int)allianceMembership;
            canApprove = num > 2;
            canKick = num > 4;
            canView = num > 2;
            canDeployCrew = num > 2;
            canMoveRoom = num > 3;
            canBuyAmmo = num > 3;
            canEditAI = num > 3;
            canDiscardAmmo = num > 4;
            canRemoveRoom = num > 4;
            canDeployItem = num > 4;
            canBuildRoom = num > 4;
            canPaint = num > 4;
            canSpeedUpAmmo = num > 4;
            canSpeedUpBuild = num > 5;
            canUpgradeShip = num > 5;
            canResearch = num > 5;
            canEditAlliance = num > 5;
        }
    }
}