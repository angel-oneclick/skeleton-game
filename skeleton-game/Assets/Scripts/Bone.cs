/*
 * Copyright © 2021+ Oneclick
 * Created in December 2021
 * by Ángel
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bone
{

    public enum Id
    {
        clavicle,           //
        scapula,            //
        sternum,            //
        humerus,            //
        ulna,               //
        radius,             //
        ilium,              //
        pubis,              //
        sacrum,             //
        coccyx,             //
        ischium,            //
        carpals,            //
        metacarpals,        //
        hands_phalanges,    //
        femur,              //
        patella,            //
        tibia,              //
        fibula,             //
        calcaneus,          //
        maxilla,            //
        hyoid,              //
        metatarsals,        //
        talus,
    }

    public static readonly List<string> names = new List<string>
    {
        "clavicle",
        "scapula",
        "sternum",
        "humerus",
        "ulna",
        "radius",
        "ilium",
        "pubis",
        "sacrum",
        "coccyx",
        "ischium",
        "carpals",
        "metacarpals",
        "hands phalanges",
        "femur",
        "patella",
        "tibia",
        "fibula",
        "calcaneus",
        "maxilla",
        "hyoid",
        "metatarsals",
        "talus",
    };

    public struct Name
    { 
        public static readonly Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>
        {
            {
                "en",
                names
            },
            {
                "es",
                new List<string>
                {
                    "clavícula",                // clavicle
                    "escápula",                 // scapula
                    "esternón",                 // sternum
                    "húmero",                   // humerus
                    "cúbito",                   // ulna
                    "radio",                    // radius
                    "ilion",                    // ilium
                    "pubis",                    // pubis
                    "sacro",                    // sacrum
                    "coxis",                    // coccyx
                    "isquión",                  // ischium
                    "carpo",                    // carpals
                    "metacarpos",               // metacarpals
                    "falanges de las manos",    // hands phalanges
                    "fémur",                    // femur
                    "rótula",                   // patella
                    "tibia",                    // tibia
                    "peroné",                   // fibula
                    "calcáneo",                 // calcaneus
                    "maxilar",                  // maxilla
                    "hioides",                  // hyoid
                    "metatarso",                // metatarsals
                    "astrálago",                // talus
                }
            }
        };
    }

}
