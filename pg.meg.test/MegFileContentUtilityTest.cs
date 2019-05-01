using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pg.meg.utility;

namespace pg.meg.test
{
    [TestClass]
    public class MegFileContentUtilityTest
    {
        private const string DATA_PATH_REG_EX = @"(DATA/)(.*)";
        
        [TestMethod]
        [DataRow("I:\\Workspace\\yvaw development\\data\\xml\\yvaw\\projectiles\\space\\proj_snubcraft_laser.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_warheads_plasmatorp.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_capital_laser.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_capital_massdriver.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_capital_turboion.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_capital_turbolaser.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_capital_turbomaser.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_snubcraft_ion.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_snubcraft_laser.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_snubcraft_massdriver.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_warheads_concussionmissile.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\projectiles\\space\\proj_warheads_magpulsemissile.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\x m l\\yvaw\\units\\space\\generic\\u_fighter_tiein.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_capital_isd2.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_capital_mc80independence.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_capital_mc80liberty.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_corvette_bayonet.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_corvette_cg273.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_corvette_cr90.xml")]
        [DataRow("I:\\Workspace\\yvaw-developmENT\\DATA\\XML\\YVAW\\UNITs\\space\\generic\\u_corvette_dp20.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_corvette_icc.xml")]
        [DataRow("I:\\Workspace/yvaw-development/data/xml/yvaw/units/space/generic/u_corvette_ipv1.xml")]
        [DataRow("I:/Workspace/yvaw-development/data/xml/yvaw/units/space/generic\\u_corvette_lancer.xml")]
        [DataRow("I:\\Workspace\\yvaw development\\data\\xml\\yvaw\\units\\space\\generic\\u_corvette_tartan.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\GENERIC\\U_CRUISER_MC40A.Xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_cruiser_vindicator.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_fighter_awing.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_fighter_bwing.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_fighter_preybird.xml")]
        [DataRow("I:\\Workspace\\yvaw-development\\data\\xml\\yvaw\\units\\space\\generic\\u_fighter_tied.xml")]
        public void ExtractFileNameForMegFileTest(string s)
        {
            Regex regularExpression = new Regex(DATA_PATH_REG_EX);
            string generatedPath = MegFileContentUtility.ExtractFileNameForMegFile(s);
            Match match = regularExpression.Match(generatedPath);
            Assert.IsTrue(match.Success);
        }
    }
}