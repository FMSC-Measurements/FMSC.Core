﻿using System;

namespace FMSC.Core
{
    public static class Convert
    {
        #region Coeff
        private const double HA_Coeff = 2.471;

        private const double FeetToMeters_Coeff = 1200d / 3937d;
        private const double YardsToMeters_Coeff = FeetToMeters_Coeff * 3d;
        private const double ChainsToMeters_Coeff = FeetToMeters_Coeff * 22d;

        private const double MetersToFeet_Coeff = 3937d / 1200d;
        private const double YardsToFeet_Coeff = 3d;
        private const double ChainsToFeet_Coeff = 66d;

        private const double FeetToYards_Coeff = 1d / 3d;
        private const double MetersToYards_Coeff = 1d / YardsToMeters_Coeff;
        private const double ChainsToYards_Coeff = 22d;

        private const double FeetToChains_Coeff = 1d / 66d;
        private const double MetersToChains_Coeff = MetersToFeet_Coeff / 66d;
        private const double YardsToChains_Coeff = 3d / 66d;


        //private const double CubicInchToBoardFoot_Coeff = 1d / 144d;
        private const double CubicFootToBoardFoot_Coeff = 12d;
        private static readonly double CubicMeterToBoardFoot_Coeff = Math.Pow(MetersToFeet_Coeff, 3) * 12d;

        //private const double BoardFootToCubicInch_Coeff = 144d;
        //private const double CubicFootToCubicInch_Coeff = 1728d;
        //private static readonly double CubicMeterToCubicInch_Coeff = Math.Pow(MetersToFeet_Coeff, 3) * 144d;

        private const double BoardFootToCubicFoot_Coeff = 1d / 12d;
        //private const double CubicInchToCubicFoot_Coeff = 1d / 1728d;
        private static readonly double CubicMeterToCubicFoot_Coeff = Math.Pow(MetersToFeet_Coeff, 3);

        private static readonly double BoardFootToCubicMeter_Coeff = 1d / CubicMeterToBoardFoot_Coeff;
        //private static readonly double CubicInchToCubicMeter_Coeff = 1d / CubicMeterToCubicInch_Coeff;
        private static readonly double CubicFootToCubicMeter_Coeff = 1d / CubicMeterToCubicFoot_Coeff;



        private const double Meters2ToAcres_Coeff = 0.00024711;
        private const double Meters2ToHectares_Coeff = 0.0001;

        private const double Degrees2Radians_Coeff = Math.PI / 180.0;
        private const double Radians2Degrees_Coeff = 180.0 / Math.PI;
        #endregion

        public static double Distance(double distance, Distance to, Distance from)
        {
            if (to == from)
                return distance;

            switch (to)
            {
                case Core.Distance.FeetTenths:
                    return ToFeetTenths(distance, from);
                case Core.Distance.Chains:
                    return ToChains(distance, from);
                case Core.Distance.Meters:
                    return ToMeters(distance, from);
                case Core.Distance.Yards:
                    return ToYards(distance, from);
            }

            throw new Exception("Invalid Option");
        }

        public static double DistanceLatLngInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double r = 6371d; // Radius of the earth in km
            double dLat = Degrees2Radians_Coeff * (lat2 - lat1);
            double dLon = Degrees2Radians_Coeff * (lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(Degrees2Radians_Coeff * (lat1)) *
                            Math.Cos(Degrees2Radians_Coeff * (lat2)) *
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);


            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            double dist = r * c; // Distanceance in km

            return dist * 1000d;
        }

        public static double ToFeetTenths(double distance, Distance distType)
        {
            switch (distType)
            {
                case Core.Distance.FeetTenths:
                    return distance;
                case Core.Distance.Meters:
                    return distance * MetersToFeet_Coeff;
                case Core.Distance.Yards:
                    return distance * YardsToFeet_Coeff;
                case Core.Distance.Chains:
                    return distance * ChainsToFeet_Coeff;
            }

            throw new Exception("Invalid Option");
        }

        public static double ToYards(double distance, Distance distType)
        {
            switch (distType)
            {
                case Core.Distance.FeetTenths:
                    return distance * FeetToYards_Coeff;
                case Core.Distance.Meters:
                    return distance * MetersToYards_Coeff;
                case Core.Distance.Yards:
                    return distance;
                case Core.Distance.Chains:
                    return distance * ChainsToYards_Coeff;
            }

            throw new Exception("Invalid Option");
        }

        public static double ToMeters(double distance, Distance distType)
        {
            switch (distType)
            {
                case Core.Distance.FeetTenths:
                    return distance * FeetToMeters_Coeff;
                case Core.Distance.Meters:
                    return distance;
                case Core.Distance.Yards:
                    return distance * YardsToMeters_Coeff;
                case Core.Distance.Chains:
                    return distance * ChainsToMeters_Coeff;
            }

            throw new Exception("Invalid Option");
        }

        public static double ToChains(double distance, Distance distType)
        {
            switch (distType)
            {
                case Core.Distance.FeetTenths:
                    return distance * FeetToChains_Coeff;
                case Core.Distance.Meters:
                    return distance * MetersToChains_Coeff;
                case Core.Distance.Yards:
                    return distance * YardsToChains_Coeff;
                case Core.Distance.Chains:
                    return distance;
            }

            throw new Exception("Invalid Option");
        }


        public static double Volume(double volume, Volume to, Volume from)
        {
            if (to == from)
                return volume;

            switch (to)
            {
                case Core.Volume.BoardFoot: return ToBoardFeet(volume, from);
                //case Core.Volume.CubicInch: return ToCubicInch(volume, from);
                case Core.Volume.CubicFoot: return ToCubicFoot(volume, from);
                case Core.Volume.CubicMeter: return ToCubicMeter(volume, from);
            }

            throw new Exception("Invalid Option");
        }

        public static double ToBoardFeet(double volume, Volume volType)
        {
            switch (volType)
            {
                case Core.Volume.BoardFoot: return volume;
                //case Core.Volume.CubicInch: return CubicInchToBoardFoot_Coeff * volume;
                case Core.Volume.CubicFoot: return  CubicFootToBoardFoot_Coeff * volume;
                case Core.Volume.CubicMeter: return CubicMeterToBoardFoot_Coeff * volume;
            }

            throw new Exception("Invalid Option");
        }

        //public static double ToCubicInch(double volume, Volume volType)
        //{
        //    switch (volType)
        //    {
        //        case Core.Volume.BoardFoot: return BoardFootToCubicInch_Coeff * volume;
        //        case Core.Volume.CubicInch: return volume;
        //        case Core.Volume.CubicFoot: return CubicFootToCubicInch_Coeff * volume;
        //        case Core.Volume.CubicMeter: return CubicMeterToCubicInch_Coeff * volume;
        //    }

        //    throw new Exception("Invalid Option");
        //}

        public static double ToCubicFoot(double volume, Volume volType)
        {
            switch (volType)
            {
                case Core.Volume.BoardFoot: return BoardFootToCubicFoot_Coeff * volume;
                //case Core.Volume.CubicInch: return CubicInchToCubicFoot_Coeff * volume;
                case Core.Volume.CubicFoot: return volume;
                case Core.Volume.CubicMeter: return CubicMeterToCubicFoot_Coeff * volume;
            }

            throw new Exception("Invalid Option");
        }

        public static double ToCubicMeter(double volume, Volume volType)
        {
            switch (volType)
            {
                case Core.Volume.BoardFoot: return BoardFootToCubicMeter_Coeff * volume;
                //case Core.Volume.CubicInch: return CubicInchToCubicMeter_Coeff * volume;
                case Core.Volume.CubicFoot: return CubicFootToCubicMeter_Coeff * volume;
                case Core.Volume.CubicMeter: return volume;
            }

            throw new Exception("Invalid Option");
        }


        public static double DegreesToPercent(double degrees)
        {
            return Math.Tan(DegreesToRadians(degrees)) * 100;
        }

        public static double PercentToDegrees(double percent)
        {
            return RadiansToDegrees(Math.Atan(percent / 100d));
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Degrees2Radians_Coeff;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * Radians2Degrees_Coeff;
        }


        public static double HectaAcresToAcres(double hectaAcres)
        {
            return hectaAcres * HA_Coeff;
        }
        

        public static double MetersSquaredToHa(double m2)
        {
            return m2 * Meters2ToHectares_Coeff;
        }

        public static double MetersSquaredToAcres(double m2)
        {
            return m2 * Meters2ToAcres_Coeff;
        }

        public static double? Angle(double? angle, Slope to, Slope from)
        {
            if (angle == null)
                return null;
            return Angle((double)angle, to, from);
        }

        public static double Angle(double angle, Slope to, Slope from)
        {
            if (to == from)
                return angle;

            if (to == Slope.Degrees)
            {
                if (from == Slope.Percent)
                {
                    return PercentToDegrees(angle);
                }
            }
            else
            {
                if (from == Slope.Degrees)
                {
                    return DegreesToPercent(angle);
                }
            }

            return angle;
        }
    }
}
