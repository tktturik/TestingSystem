using Authorization;
using Authorization.EndPoints;

namespace TestAuthDLL
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }
        public static IEnumerable<object[]> TestUserIds
        {
            get
            {
                yield return new object[] { 1 };
                yield return new object[] { 2 };
                yield return new object[] { 3 };
                yield return new object[] { 5 };
                yield return new object[] { -1 };
            }
        }
        [TestMethod]
        public async Task GetUsersList()
        {
            Auth authorization = new Auth();
            Dictionary<int,string> result = await authorization.GetUsers();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            foreach (KeyValuePair<int, string> user in result)
            {
                TestContext.WriteLine($"Телефон: {user.Key}, ФИО: {user.Value}");
            }

        }
        [TestMethod]
        [DynamicData(nameof(TestUserIds))]
        public async Task SendCodeWithOutToken(int _id)
        {
            Auth authorization = new Auth();

            var (code, response) = await authorization.SendCodeTG(_id);
            Assert.IsNotNull(code);
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Length == 6);

            Assert.IsTrue(int.TryParse(code, out int numericCode));
            Assert.IsTrue(numericCode >= 0);
            Assert.IsNotNull(response);
            Assert.AreEqual(response, "success");
            await Task.Delay(1000); 

        }
        [TestMethod]
        [DynamicData(nameof(TestUserIds))]
        public async Task SendCodeWithToken(int _id)
        {
            Auth authorization = new Auth();

            authorization.TempToken = await authorization.CallEndpointAsync(new TemporaryToken());

            var (code, response )= await authorization.SendCodeTG(_id);
            Assert.IsNotNull(code);
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Length == 6);

            Assert.IsTrue(int.TryParse(code, out int numericCode));
            Assert.IsTrue(numericCode >= 0);
            Assert.IsNotNull(response);
            Assert.AreEqual(response, "success");
        }
        [TestMethod]
        [DynamicData(nameof(TestUserIds))]
        public async Task VerifyOneWithToken(int _id)
        {
            Auth authorization = new Auth();
            authorization.TempToken = await authorization.CallEndpointAsync(new TemporaryToken());
            var (token, user) = await authorization.Verify(_id);
            TestContext.WriteLine((String)token);
            Assert.IsNotNull(token);
            Assert.IsNotNull(user);
        } 
        [TestMethod]
        [DynamicData(nameof(TestUserIds))]
        public async Task VerifyOneWithOutToken(int _id)
        {
            Auth authorization = new Auth();
            var (token, user) = await authorization.Verify(_id);
            Assert.IsNotNull(token);
            Assert.IsNotNull(user);
        }
    }
}