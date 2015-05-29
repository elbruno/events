// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************

using System;

namespace Oltiva.Azure.FaceApi.Contract
{
    /// <summary>
    /// The class for similar face.
    /// </summary>
    public class SimilarFace
    {
        /// <summary>
        /// Gets or sets the face identifier.
        /// </summary>
        /// <value>
        /// The face identifier.
        /// </value>
        public Guid FaceId { get; set; }
    }
}
