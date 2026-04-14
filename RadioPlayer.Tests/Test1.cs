namespace RadioPlayer.Tests {
    [TestClass]
    [DeploymentItem("TestData")]
    public sealed class Test1 {
        [TestMethod(DisplayName = "2 + 2 = 4")]
        [Description("Test de base pour tester les tests unitaires en C#")]
        public void TestMethod1() {
            int expected = 4;
            int actual = 2 + 2;
            Assert.AreEqual(expected, actual, "2 + 2 ça fait 4");
        }
    }
}
