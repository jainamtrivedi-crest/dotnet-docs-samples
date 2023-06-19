// Copyright 2023 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License

using Google.Cloud.Dlp.V2;
using Newtonsoft.Json;
using Xunit;

namespace GoogleCloudSamples
{
    public class DeidentifyTableWithCryptoHashTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public DeidentifyTableWithCryptoHashTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestDeidentifyCryptoHash()
        {
            var result = DeidentifyTableWithCryptoHash.Deidentify(_fixture.ProjectId);
            var serializedObject = result.ToString();
            Assert.DoesNotContain("user1@example.org", serializedObject); // user1@example.org is email address in the table.
            Assert.DoesNotContain("858-555-0222", serializedObject); // 858-555-0222 is the phone number in the table.
        }
    }
}
