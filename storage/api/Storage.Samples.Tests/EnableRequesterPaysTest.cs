﻿// Copyright 2020 Google Inc.
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
// limitations under the License.

using System;
using Xunit;

[Collection(nameof(BucketFixture))]
public class EnableRequesterPaysTest
{
    private readonly BucketFixture _bucketFixture;

    public EnableRequesterPaysTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestEnableRequesterPays()
    {
        EnableRequesterPaysSample enableRequesterPaysSample = new EnableRequesterPaysSample();
        DisableRequesterPaysSample disableRequesterPaysSample = new DisableRequesterPaysSample();
        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        _bucketFixture.CreateBucket(bucketName);

        // Enable request pay.
        var bucket = enableRequesterPaysSample.EnableRequesterPays(bucketName);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        Assert.True(bucket.Billing?.RequesterPays);

        // Disable request pay.
        disableRequesterPaysSample.DisableRequesterPays(_bucketFixture.ProjectId, bucketName);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
