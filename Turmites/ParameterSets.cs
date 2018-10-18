using System.Collections.Generic;
using WorldSystem;

public static class ParameterSets
{
    public static List<Parameter> Default()
    {
        return new List<Parameter>
        {
            new Parameter("States", 2, 2, 8),
            new Parameter("Source", "1OX2R,1XO2R,2OX1S,2XX2S")
        };
    }

    public static List<TurmiteCode> TurmiteLibrary = new List<TurmiteCode>();

    // Adapted from https://github.com/rm-hull/turmites
    public static void InitializeTurmiteLibrary()
    {
        //Default
        TurmiteLibrary.Add(new TurmiteCode("Default", "1OX2R,1XO2R,2OX1S,2XX2S"));

        //langtons-ant "120 080"
        TurmiteLibrary.Add(new TurmiteCode("Langton-Ant", "1OX1R,1XO1L"));

        //fibonacci "181 181 121 010"
        TurmiteLibrary.Add(new TurmiteCode("Fibonacci", "1OX2L,1XX2L,2OX2R,2XO1S"));

        //binary-counter "120 010"
        TurmiteLibrary.Add(new TurmiteCode("Binary Counter", "1OX1R,1XO1S"));

        //filled-triangle "081 121 110 111"
        TurmiteLibrary.Add(new TurmiteCode("Filled Triangle", "1OO2L,1XX2R,2OX1S,2XX2S"));

        //box-spiral "011 081 120 011"
        TurmiteLibrary.Add(new TurmiteCode("Box Spiral", "1OO2S,1XO2L,2OX1R,2XO2S"));

        //striped-spiral "021 080 181 020"
        TurmiteLibrary.Add(new TurmiteCode("Striped Spiral", "1OO2R,1XO1L,2OX2L,2XO1R"));

        //stepped-pyramid "021 080 181 110"
        TurmiteLibrary.Add(new TurmiteCode("Stepped Pyramid", "1OO2R,1XO1L,2OX2L,2XX1S"));

        //contoured-island "021 011 121 180"
        TurmiteLibrary.Add(new TurmiteCode("Contoured Island", "1OO2R,1XO2S,2OX2R,2XX1L"));

        //woven-placemat "021 021 110 021"
        TurmiteLibrary.Add(new TurmiteCode("Woven Placemat", "1OO2R,1XO2R,2OX1S,2XO2R"));

        //snowflake-ish "021 121 181 180"
        TurmiteLibrary.Add(new TurmiteCode("Snowflake-Like", "1OO2R,1XX2R,2OX2L,2XX1L"));

        //slow-city-builder "180 011 080 081"
        TurmiteLibrary.Add(new TurmiteCode("Slow City Builder", "1OX1L,1XO2S,2OO1L,2XO2L"));

        //computer-art "180 121 020 081"
        TurmiteLibrary.Add(new TurmiteCode("Computer Art", "1OX1L,1XX2R,2OO1R,2XO2L"));

        //ballon-bursting "180 121 021 180"
        TurmiteLibrary.Add(new TurmiteCode("Balloon Bursting", "1OX1L,1XX2R,2OO2R,2XX1L"));

        //horizontal-highway "181 080 110 010"
        TurmiteLibrary.Add(new TurmiteCode("Horizontal Highway", "1OX2L,1XO1L,2OX1S,2XO1S"));

        //highway1 "181 080 121 120"
        TurmiteLibrary.Add(new TurmiteCode("Highway 1", "1OX2L,1XO1L,2OX2R,2XX1R"));

        //filled-spiral "181 010 110 120"
        TurmiteLibrary.Add(new TurmiteCode("Filled Spiral", "1OX2L,1XO2S,2OX1S,2XX1R"));

        //glaciers "181 020 080 080"
        TurmiteLibrary.Add(new TurmiteCode("Glaciers", "1OX2L,1XO1R,2OO1L,2XO1L"));

        //fizzy-spill "181 120 080 080"
        TurmiteLibrary.Add(new TurmiteCode("Fizzy Spill", "1OX2L,1XX1R,2OO1L,2XO1L"));

        //nested-cabinets "181 121 110 011"
        TurmiteLibrary.Add(new TurmiteCode("Nested Cabinets", "1OX2L,1XX2R,2OX1S,2XO2S"));

        //cross "111 081 120 111"
        TurmiteLibrary.Add(new TurmiteCode("Cross", "1OX2S,1XO2L,2OX1R,2XX2S"));

        //saw "111 010 020 180"
        TurmiteLibrary.Add(new TurmiteCode("Saw", "1OX2S,1XO1S,2OO1R,2XX1L"));

        //curves-in-blocks "111 011 121 010"
        TurmiteLibrary.Add(new TurmiteCode("Curves in Blocks", "1OX2S,1XO2S,2OX2R,2XO1S"));

        //textured "111 020 080 080"
        TurmiteLibrary.Add(new TurmiteCode("Textured", "1OX2S,1XO1R,2OO1L,2XO1L"));

        //diamond "111 021 180 120"
        TurmiteLibrary.Add(new TurmiteCode("Diamond", "1OX2S,1XO2R,2OX1L,2XX1R"));

        //coiled-rope "111 180 121 010"
        TurmiteLibrary.Add(new TurmiteCode("Coiled Rope", "1OX2S,1XX1L,2OX2R,2XO1S"));

        //growth1 "120 081 180 011"
        TurmiteLibrary.Add(new TurmiteCode("Growth 1", "1OX1R,1XO2L,2OX1L,2XO2S"));

        //square-spiral "120 081 180 021"
        TurmiteLibrary.Add(new TurmiteCode("Square Spiral", "1OX1R,1XO2L,2OX1L,2XO2R"));

        //loopy "120 121 010 011"
        TurmiteLibrary.Add(new TurmiteCode("Loopy", "1OX1R,1XX2R,2OO1S,2XO2S"));

        //square-ant "121 081 110 010"
        TurmiteLibrary.Add(new TurmiteCode("Square Ant", "1OX2R,1XO2L,2OX1S,2XO1S"));

        //distracted-spiral "121 020 010 121"
        TurmiteLibrary.Add(new TurmiteCode("Distracted Spiral", "1OX2R,1XO1R,2OO1S,2XX2R"));

        //cauliflower-stalk "121 021 110 111"
        TurmiteLibrary.Add(new TurmiteCode("Cauliflower Stalk", "1OX2R,1XO2R,2OX1S,2XX2S"));

        //worm-trails "121 181 121 020"
        TurmiteLibrary.Add(new TurmiteCode("Worm Trails", "1OX2R,1XX2L,2OX2R,2XO1R"));

        //two-way-highway "121 110 110 011"
        TurmiteLibrary.Add(new TurmiteCode("Two-Way Highway", "1OX2R,1XX1S,2OX1S,2XO2S"));

        //mould-bloom "121 120 010 010"
        TurmiteLibrary.Add(new TurmiteCode("Mold Bloom", "1OX2R,1XX1R,2OO1S,2XO1S"));

        //highway3 "121 120 020 111"
        TurmiteLibrary.Add(new TurmiteCode("Highway 3", "1OX2R,1XX1R,2OO1R,2XX2S"));

        //highway4 "121 121 181 020"
        TurmiteLibrary.Add(new TurmiteCode("Highway 4", "1OX2R,1XX2R,2OX2L,2XO1R"));

        //square-diagonal "021 121 082 080 122 180"
        TurmiteLibrary.Add(new TurmiteCode("Square Diagonal", "1OO2R,1XX2R,2OO3L,2XO1L,3OX3R,3XX1L", 3));

        //streak2 "181 012 022 111 121 110"
        TurmiteLibrary.Add(new TurmiteCode("Streak 2", "1OX2L,1XO3S,2OO3R,2XX2S,3OX2R,3XX1S", 3));

        //maze "181 181 110 012 081 111"
        TurmiteLibrary.Add(new TurmiteCode("Maze", "1OX2L,1XX2L,2OX1S,2XO3S,3OO2L,3XX2S", 3));

        //cornices "182 020 180 020 080 081"
        TurmiteLibrary.Add(new TurmiteCode("Cornices", "1OX3L,1XO1R,2OX1L,2XO1R,3OX1L,3XO2L", 3));

        //highway5 "120 022 080 020 011 180"
        TurmiteLibrary.Add(new TurmiteCode("Highway 5", "1OX1R,1XO3R,2OO1L,2XO1R,3OO2S,3XX1L", 3));

        //highway6 "121 080 122 010 180 080"
        TurmiteLibrary.Add(new TurmiteCode("Highway 6", "1OX2R,1XO1L,2OX3R,2XO1S,3OX1L,3XO1L", 3));
    }
}
