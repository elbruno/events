// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************

using System;

namespace Oltiva.Azure.FaceApi.Contract
{
    /// <summary>
    /// The class for person creation result.
    /// </summary>
    public class CreatePersonResult
    {
        /// <summary>
        /// Gets or sets the person identifier.
        /// </summary>
        /// <value>
        /// The person identifier.
        /// </value>
        public Guid PersonId { get; set; }
    }
}
