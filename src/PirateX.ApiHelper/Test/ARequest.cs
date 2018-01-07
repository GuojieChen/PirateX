using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PirateX.Core.Actor;

namespace PirateX.ApiHelper.Test
{
    public class ARequest:RepAction<ARequestResponse>
    {
        public override ARequestResponse Play()
        {
            throw new NotImplementedException();
        }
    }

    public class ARequestResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}