﻿using System;

namespace Sygic.Corona.Domain
{
    public class Location //Entity
    {
        public Guid Id { get; set; }
        public long ProfileId { get; set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public double? Accuracy { get; private set; }
        public DateTime CreatedOn { get; private set; }

        protected Location()
        {
            CreatedOn = DateTime.UtcNow;
        }
        public Location(double? latitude, double? longitude, double? accuracy) : this()
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
        }

        public Location(double latitude, double longitude, double accuracy) : this()
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
        }

        public Location(uint profileId, double? latitude, double? longitude, double? accuracy) : this()
        {
            ProfileId = profileId;
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
        }

        public Location(uint profileId, double? latitude, double? longitude, double? accuracy, DateTime createdOn)
        {
            ProfileId = profileId;
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
            CreatedOn = createdOn;
        }
    }
}
