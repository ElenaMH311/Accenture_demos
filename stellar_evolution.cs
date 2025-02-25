using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Diagnostics; //Needed for the stopwatch 
using System.Text; 
using System.Threading.Tasks; 
using System.IO; //Needed for file handling 

namespace Stellar_Evolution_NEA 
{ 
    public struct starVals 
    { 
        public List<string> starNames; 
        public List<double> starMasses; 
    } 

    class Program 
    { 
        static void Main(string[] args) 
        { 
            bool first = true; 
            int currentStar = -1; 
            int[,] position = new int[10, 3]; 
            List<string> nameList = new List<string>(); 
            List<double> masses = new List<double>(); //ASSIGN MASSES 

            Console.WriteLine("****************************"); 
            Console.WriteLine("Welcome to Stellar Evolution"); //Sets up variables and arrays at the start of programme 
            Console.WriteLine("****************************"); 
            Console.WriteLine(); 

            Menu(nameList, first, position, ref currentStar, masses); 
        } 

        public static void Menu(List<string> nameList, bool first, int[,] position, ref int currentStar, List<double> masses) 
        { 
            //Menu is a separate method so we can return to the menu option without returning to Main and executing the start code 
            bool exit = false; 

            do 
            { 
                Console.WriteLine("1: Create new star"); 
                Console.WriteLine("2: View all stars"); 
                Console.WriteLine("3: Exit"); 

                string holdChoice = Console.ReadLine(); 
                bool validChoice = int.TryParse(holdChoice, out int menuChoice); 

                if (validChoice) 
                { 
                    switch (menuChoice) 
                    { 
                        case 1: 
                            createStar(ref first, nameList, ref position, ref currentStar, masses); 
                            break; 

                        case 2: 
                            viewStars(nameList, masses); 
                            break; 

                        case 3: 
                            exit = true; 
                            break; 

                        default: 
                            Console.WriteLine("Error! No valid choice selected"); 
                            break; 
                    } 
                } 
                else Console.WriteLine("Error! No valid choice selected"); 
            } while (!exit); //Loop used to repeat after executing choice or giving error message, so the user returns to the menu 
        } 

        public static void createStar(ref bool first, List<string> nameList, ref int[,] position, ref int currentStar, List<double> masses) 
        { 
            //This method takes in and validates all the information necessary to the star's creation 
            //This is then passed into a method to carry out the full evolution of this star 
            string name; 
            bool validName; 
            bool exit = false; 
            int starType; 
            double mass; 
            List<int> pathList = new List<int>(); 

            currentStar += 1; 
            Console.WriteLine(); 

            do 
            { 
                validName = true; 
                Console.Write("Please enter the name of your star. All star names must be unique:"); 
                name = Console.ReadLine();

                for (int i = 0; i < nameList.Count; i++) 
                { 
                    if (name == nameList[i]) 
                    { 
                        Console.WriteLine("Error! This name has already been used."); 
                        validName = false; 
                    } 
                } 
            } while (!validName); 

            nameList.Add(name); 

            Console.WriteLine(); 
            Console.WriteLine("Please enter the number corresponding to the type of star you wish to create."); 
	    Console.WriteLine("Invalid entry will mean your star will default to a nebula."); 
            Console.WriteLine("0: Nebula"); 
            Console.WriteLine("1: Protostar"); 
            Console.WriteLine("2: Brown dwarf"); 
            Console.WriteLine("3: Main sequence star"); 
            Console.WriteLine("4: Blue dwarf"); 
            Console.WriteLine("5: Red giant"); 
            Console.WriteLine("6: Red supergiant"); 
            Console.WriteLine("7: White dwarf"); 
            Console.WriteLine("8: Neutron star"); 
            Console.WriteLine("9: Black hole"); 
            Console.WriteLine("10: Black dwarf"); 

            string holdChoice = Console.ReadLine(); 
            bool validChoice = int.TryParse(holdChoice, out starType); 

            if (!validChoice) starType = 0; 
            else 
            { 
                if (starType < 0 || starType > 10) starType = 0; 
            } 
            Console.WriteLine(); 

            do 
            { 
                Console.Write("Please enter the mass of the star in Solar Masses: "); 
                holdChoice = Console.ReadLine(); 
                validChoice = double.TryParse(holdChoice, out mass); 

                if (validChoice) 
                { 
                    bool typeError = true; 
                    bool massError = false; 

                    if (mass > 0 || mass <= 60) 
                    { 
                        pathList = planPath(mass, starType); 
                        for (int i = 0; i < pathList.Count; i++) 
                        { 
                            if (pathList[i] == starType) 
                            { 
                                exit = true; 
                                typeError = false; 
                            } 
                        } 
                    } 
                    else 
                    { 
                        Console.WriteLine("Error! Mass must be between 0 and 60 SM"); 
                        massError = true; 
                    } 
                    if (typeError && !massError) Console.WriteLine("Error! Entered mass is not in range for selected star type"); 
                } 
                else Console.WriteLine("Error! Mass must be a numerical value"); 
            } while (!exit); 

            if (!first) //userPosition cannot be called for the first created star bc there are no other stars to distance from 
            { 
                Console.WriteLine("1: Plot star a set distance from another star"); 
                Console.WriteLine("2: Plot star randomly"); 
                int plot = Convert.ToInt32(Console.ReadLine()); 

                switch (plot) 
                { 
                    case 1: 
                        userPosition(ref position, nameList, currentStar); 
                        break; 

                    case 2: 
                        randomPosition(ref position, currentStar); 
                        break; 

                    default: //Defensive programming in case user does not give a valid response 
                        randomPosition(ref position, currentStar); 
                        Console.WriteLine(position); 
                        break; 
                } 
            } 

            if (first) 
            { 
                randomPosition(ref position, currentStar);
                first = false; 
            } 

            masses.Add(mass); 
            Console.WriteLine(); 

            evolveStar(ref position, name, mass, pathList, ref currentStar, nameList); 
            Console.WriteLine(); 
        } 

        public static List<int> planPath(double mass, int starType) 
        { 
            List<int> pathList = new List<int>(); 
            List<double> massBoundaries = new List<double> { 0, 0.08, 0.25, 1.4, 2.5, 60 }; //Mass boundaries 
            List<int> path1 = new List<int> { 0, 1, 2, 10 }; //Nebula, protostar, brown dwarf, black dwarf 
            List<int> path2 = new List<int> { 0, 1, 3, 4, 7, 10 }; //Nebula, protostar, MS, blue dwarf, white dwarf, black dwarf 
            List<int> path3 = new List<int> { 0, 1, 3, 5, 7, 10 }; //Nebula, protostar, MS, red giant, white dwarf, black dwarf 
            List<int> path4 = new List<int> { 0, 1, 3, 6, 8 }; //Nebula, protostar, MS, red supergiant, neutron star 
            List<int> path5 = new List<int> { 0, 1, 3, 6, 9 }; //Nebula, protostar, MS, red supergiant, black hole 
            List<List<int>> paths = new List<List<int>> { path1, path2, path3, path4, path5 }; 

            for (int i = 0; i < 5; i++) 
            { 
                if (mass > massBoundaries[i] && mass <= massBoundaries[i + 1]) 
                { //Finds which mass boundary the star is between and allocates its path accordingly 
                    foreach (int x in paths[i]) 
                    { 
                        if (x >= starType) pathList.Add(x); 
                    } //Disregards all stages of life before the stage requested by the user 
                } 
            } 
            return pathList; 
        } 

        public static void userPosition(ref int[,] position, List<string> nameList, int currentStar) 
        { 
            //inputs the name of the star to distance from and the distance away from it, cannot be zero, negatives will be converted 
            bool validName = false, validDistance = false; 
            int compStar, x, y, z, distance; 
            Random randPos = new Random(); 

            do 
            { 
                compStar = 0; //If the loop repeats, the value of this count must be reset 
                Console.WriteLine("Please enter the name of the star to measure the distance by: "); 
                string checkName = Console.ReadLine(); 

                foreach (string name in nameList) 
                { 
                    if (checkName == name) validName = true; 
                    if (!validName) compStar++; //Used to track where we are in the position list 
                } 
                if (!validName) Console.WriteLine("Error! No star exists under that name. Please enter a valid name: "); 
                //Could maybe include the option to view existing star names at this point? 
            } while (!validName); 

            do 
            { 
                Console.WriteLine("Distances must be up to 45000m"); 
                Console.WriteLine("Please enter the distance from the {0} that {1} will be: ", nameList[compStar], nameList[currentStar]); 
                distance = Convert.ToInt32(Console.ReadLine()); 
                if (distance >= -45000 && distance <= 45000 && distance != 0) validDistance = true; 
                else if (distance < -45000 || distance > 45000) Console.WriteLine("Error! Distance is out of range"); 
            } while (!validDistance); 
            Console.WriteLine(distance); 

            if (distance < 0) distance = Math.Abs(distance); //Later calculations require the distance to be positive (negatives cannot be rooted) 

            //Distance = sqrt(x^2 + y^2 + z^2)  
            distance = Convert.ToInt32(Math.Pow(distance, 2)); 
            x = Convert.ToInt32(Math.Sqrt(randPos.Next(0, distance))); 
            distance = distance - x*x; 
            y = Convert.ToInt32(Math.Sqrt(randPos.Next(0, distance))); 
            distance = distance - y*y; 
            Console.WriteLine(distance); 
            z = Convert.ToInt32(Math.Sqrt(distance)); 

            Console.WriteLine("{0}, {1}, {2}", x, y, z); 
            Console.WriteLine(compStar); 
            Console.WriteLine("{0}, {1}, {2}", position[compStar, 0], position[compStar, 1], position[compStar, 2]); 

            if (position[compStar, 0] + x < 90000) position[currentStar, 0] = position[compStar, 0] + x; 
            else position[currentStar, 0] = position[compStar, 0] - x; 
            if (position[compStar, 1] + y < 90000) position[currentStar, 1] = position[compStar, 1] + y; 
            else position[currentStar, 1] = position[compStar, 1] - y; 
            if (position[compStar, 2] + x < 90000) position[currentStar, 2] = position[compStar, 2] + z; 
            else position[currentStar, 2] = position[compStar, 2] - z; 

            Console.WriteLine("{0}, {1}, {2}", position[currentStar, 0], position[currentStar, 1], position[currentStar, 2]); 
        } 

        public static void randomPosition(ref int[,] position, int currentStar) 
        { 
            Random randPos = new Random(); 
            bool valueRepeated = false; 
            int x, y, z; 

            //Generates a random set of (x, y, z) coordinates for the star's position 
            do 
            { 
                x = randPos.Next(0, 90000); 
                y = randPos.Next(0, 90000); //This simulation models the stars as points in space and does not account for their radii 
                z = randPos.Next(0, 90000); 

                for (int i = 0; i < currentStar; i++) 
                { 
                    if (position[i, 0] == x && position[i, 1] == y && position[i, 2] == z) 
                    { 
                        valueRepeated = true; 
                    } 
                } 
            } while (valueRepeated); //Ensures no two stars are located in the same place 

            //If a value is repeated, it will loop again, generating a new value 

            position[currentStar, 0] = x; 
            position[currentStar, 1] = y; 
            position[currentStar, 2] = z; 

            Console.WriteLine("{0}, {1}, {2}", position[currentStar, 0], position[currentStar, 1], position[currentStar, 2]); 
        } 

        public static void viewStars(List<string> nameList, List<double> masses) 
        { 
            //This method references the stars database and outputs the name, birth time, death time (if applicable) and life path of all stars created within that run of the game. 

            Console.WriteLine("1: View this game's stars"); 
            Console.WriteLine("2: View a file"); 
            Console.WriteLine("3: Run a star's life cycle again"); 

            int menuChoice = Convert.ToInt32(Console.ReadLine()); 
            bool viewOne = false; 

            switch (menuChoice) 
            { 
                case 1: 
                    Console.WriteLine("Stars created this game:"); 
                    printStars(nameList); 
                    break; 

                case 2: 
                    loadFile(viewOne); 
                    break; 

                case 3: 
                    Console.WriteLine("Stars created this game: "); 
                    printStars(nameList); 
                    viewOneCycle(nameList, masses); 
                    break; 
            } 
        } 

        public static starVals mergeSort(starVals starValues) 
        { 
            starVals leftStarVals = new starVals(); 
            leftStarVals.starNames = new List<string>(); 
            leftStarVals.starMasses = new List<double>(); 
            starVals rightStarVals = new starVals(); 
            rightStarVals.starNames = new List<string>(); 
            rightStarVals.starMasses = new List<double>(); 

            if (starValues.starNames.Count > 1) 
            { 
                int mid = starValues.starNames.Count / 2; 

                for (int x = 0; x < mid; x++) 
                { 
                    leftStarVals.starNames.Add(starValues.starNames[x]); 
                    leftStarVals.starMasses.Add(starValues.starMasses[x]); 
                } 

                for (int x = mid; x < starValues.starNames.Count; x++) 
                { 
                    rightStarVals.starNames.Add(starValues.starNames[x]); 
                    rightStarVals.starMasses.Add(starValues.starMasses[x]); 
                } 
            } 
            else return starValues; //Escape condition to keep from infinite looping and stack overload 

            leftStarVals = mergeSort(leftStarVals); 
            rightStarVals = mergeSort(rightStarVals); 
            int i = 0, j = 0, k = 0, letterCount; 
            bool sorted; 

            while (i < leftStarVals.starNames.Count && j < rightStarVals.starNames.Count) 
            { 
                letterCount = 0; 
                sorted = false; 

                do 
                { 
                    if (leftStarVals.starNames[i][letterCount] < rightStarVals.starNames[j][letterCount]) 
                    { 
                        starValues.starNames[k] = leftStarVals.starNames[i]; 
                        starValues.starMasses[k] = leftStarVals.starMasses[i]; 
                        i += 1; 
                        sorted = true; 
                    } 
                    else if (leftStarVals.starNames[i][letterCount] == rightStarVals.starNames[j][letterCount]) 
                    { 
                        letterCount++; //If the start letters are the same, the loop repeats until two different letters are found and the names can be ordered 
                    } 
                    else 
                    { 
                        starValues.starNames[k] = rightStarVals.starNames[j]; 
                        starValues.starMasses[k] = rightStarVals.starMasses[j]; 
                        j += 1; 
                        sorted = true; 
                    } 
                } while (!sorted);  //If the start letters are the same, sorted will be false and the loop will repeat and compare the next letters. 
                k += 1; 
            } 

            while (i < leftStarVals.starNames.Count) 
            { 
                starValues.starNames[k] = leftStarVals.starNames[i]; 
                starValues.starMasses[k] = leftStarVals.starMasses[i]; 
                i += 1; 
                k += 1; 
            } 

            while (j < rightStarVals.starNames.Count) 
            { 
                starValues.starNames[k] = rightStarVals.starNames[j]; 
                starValues.starMasses[k] = rightStarVals.starMasses[j]; 
                j += 1; 
                k += 1; 
            } 
            return starValues; 
        } 

        public static void printStars(List<string> nameList) 
        { 
            for (int i = 0; i < nameList.Count; i++) 
                Console.WriteLine(nameList[i]); 
        } 

        public static void loadFile(bool viewOne) 
        { 
            //LOAD THE NAMES FROM THE FILE AND STORE THE NAME LIST AND MASSES IN TWO LISTS 
            //THEN IF WE'RE VIEWING ONE, PASS THESE LISTS TO THE VIEWONECYCLE METHOD 

            string fileName, namesLine, massesLine, holdChoice; 
            List<string> holdMasses = new List<string>(); 
            int menuChoice; 

            starVals orderVals = new starVals(); 
            orderVals.starNames = new List<string>(); 
            orderVals.starMasses = new List<double>(); 

            Console.WriteLine("Please enter the name of the file: "); 
            fileName = Console.ReadLine(); 

            try 
            { 
                StreamReader openFile = new StreamReader(fileName); 
                namesLine = openFile.ReadLine(); 
                massesLine = openFile.ReadLine(); 
                orderVals.starNames = namesLine.Split(',').ToList(); 
                holdMasses = massesLine.Split(',').ToList(); 

                for (int i = 0; i < holdMasses.Count; i++) 
                { 
                    orderVals.starMasses.Add(Convert.ToDouble(holdMasses[i])); 
                } 

                orderVals = mergeSort(orderVals); 

                for (int i = 0; i < orderVals.starNames.Count; i++) 
                { 
                    Console.WriteLine("{0}, {1}", orderVals.starNames[i], orderVals.starMasses[i]); 
                } 

                Console.WriteLine("1: View a star's life cycle"); 
                Console.WriteLine("2: Return to main menu"); 

                holdChoice = Console.ReadLine(); 
                bool validChoice = int.TryParse(holdChoice, out menuChoice); 

                if (menuChoice == 1) viewOneCycle(orderVals.starNames, orderVals.starMasses); 
                if (!validChoice || menuChoice != 2) Console.WriteLine("No valid response given. Returning to menu..."); 
            } 
            catch 
            { 
                Console.WriteLine("Error! No file can be found under that name"); 
            } 
            Console.WriteLine(); 
        } 

        public static void viewOneCycle(List<string> starNames, List<double> starMasses) 
        //This method uses a binary sort, so it assumes that starNames is alphabetical 
        //starNames must be sorted before it is passed in 

        { 
            int[,] position = new int[starNames.Count, 3]; 
            printStars(starNames); 
            Console.WriteLine("Please enter the name of the star you wish to run: "); 
            string starChoice = Console.ReadLine(); 
            int listPoint = binarySearch(starNames, starChoice, 0, starNames.Count, 0); 
 
            if (listPoint == 0) 
            { 
                if (starNames[0] == starChoice) 
                { 
                    randomPosition(ref position, listPoint); 
                    List<int> pathList = planPath(starMasses[0], 0); 
                    evolveStar(ref position, starNames[0], starMasses[0], pathList, ref listPoint, starNames); 
                } 
                else Console.WriteLine("Error! Star name could not be found"); 
            } 
            else 
            { 
                randomPosition(ref position, listPoint); 
                Console.WriteLine("{0}", position[listPoint, 0]); 
                List<int> pathList = planPath(starMasses[listPoint], 0); 
                evolveStar(ref position, starNames[listPoint], starMasses[listPoint], pathList, ref listPoint, starNames); 
            } 
        } 

        public static int binarySearch(List<string> starNames, string starChoice, int first, int last, int letterCount) 
        { 
            int midpoint; 

            if(starNames[first][letterCount] > starNames[last][letterCount]) return 0; 
            else 
            { 
                midpoint = (first + last) / 2; 
                if (starNames[midpoint][letterCount] > starChoice[letterCount]) return binarySearch(starNames, starChoice, first, midpoint - 1, letterCount); 
                else if (starNames[midpoint][letterCount] < starChoice[letterCount]) return binarySearch(starNames, starChoice, midpoint + 1, last, letterCount); 
                else 
                { 
                    if (starNames[midpoint] == starChoice) 
                    { 
                        return midpoint; 
                    } 
                    else return binarySearch(starNames, starChoice, first, last, letterCount++); 
                } 
            } 
        } 

        public static void evolveStar(ref int[,] position, string name, double mass, List<int> pathList, ref int currentStar, List<string> nameList) 
        { 
            double timeToElapse = 0; 
            Stopwatch gameTime = new Stopwatch(); 
            gameTime.Start(); 

            for (int i = 0; i < pathList.Count; i++) 
            { 
                switch (pathList[i]) 
                { 
                    case 0: 
                        Birth Nebula = new Birth(); 
                        Nebula.setName(name); 
                        Nebula.printEvolution(); 
                        timeToElapse += Nebula.timeToNextPhase(); 
                        break; 

                    case 1: 
                        Development Protostar = new Development(); 
                        Protostar.setName(name); 
                        Protostar.printEvolution(); 
                        timeToElapse += Protostar.timeToNextPhase(); 
                        break; 

                    case 2: 
                        BrownDwarf brownDwarf = new BrownDwarf(); 
                        brownDwarf.setName(name); 
                        brownDwarf.printEvolution(); 
                        timeToElapse += brownDwarf.timeToNextPhase(); 
                        break; 

                    case 3: 
                        MainSequence mainSeq = new MainSequence(); 
                        mainSeq.setName(name); 
                        mainSeq.setMass(mass); 
                        mainSeq.printEvolution(); 
                        mainSeq.setSpectralClass(); 
                        Console.WriteLine("Spectral class is {0}", mainSeq.getSpectralClass()); 
                        Console.WriteLine("{0} is {1}", mainSeq.getName(), mainSeq.getColour()); 
                        timeToElapse += mainSeq.timeToNextPhase(); 
                        break; 

                    case 4: 
                        BlueDwarf blueDwarf = new BlueDwarf(); 
                        blueDwarf.setName(name); 
                        blueDwarf.printEvolution(); 
                        timeToElapse += blueDwarf.timeToNextPhase(); 
                        break; 

                    case 5: 
                        RedGiant redGiant = new RedGiant(); 
                        redGiant.setName(name); 
                        redGiant.printEvolution(); 
                        timeToElapse += redGiant.timeToNextPhase(); 
                        break; 

                    case 6: 
                        RedSupergiant redSG = new RedSupergiant(); 
                        redSG.setName(name); 
                        redSG.printEvolution(); 
                        timeToElapse += redSG.timeToNextPhase(); 
                        break; 

                    case 7: 
                        WhiteDwarf wDwarf = new WhiteDwarf(); 
                        wDwarf.setName(name); 
                        wDwarf.printEvolution(); 
                        timeToElapse += wDwarf.timeToNextPhase(); 
                        break; 

                    case 8: 
                        NeutronStar nStar = new NeutronStar(); 
                        nStar.setName(name); 
                        nStar.printEvolution(); 
                        timeToElapse += nStar.timeToNextPhase(); 
                        break; 

                    case 9: 
                        BlackHole blackHole = new BlackHole(); 
                        blackHole.setName(name); 
                        blackHole.setMass(mass); 
                        blackHole.setPosition(position, currentStar); 
                        blackHole.printEvolution(); 
                        blackHole.setSchRadius(); 
                        Console.WriteLine("{0} has a Schwarzschild radius of {1}m", blackHole.getName(), blackHole.getSchRadius()); 
                        blackHole.inSchRad(nameList, position); 
                        timeToElapse += blackHole.timeToNextPhase(); 
                        break; 

                    case 10: 
                        Death3 blackDwarf = new Death3(); 
                        blackDwarf.setName(name); 
                        blackDwarf.printEvolution(); 
                        timeToElapse += blackDwarf.timeToNextPhase(); 
                        break; 
                } 

                double timeConvertor = timeToElapse / 100; 
                bool timePrinted = false; 

		do 
                { 
                    if (gameTime.ElapsedMilliseconds == timeToElapse && !timePrinted) 
                    { 
                        Console.WriteLine("{0} has evolved at {1} million years", name, timeConvertor); 
                        timePrinted = true; 
                    } 
                } while (gameTime.ElapsedMilliseconds < timeToElapse); 

                if (!timePrinted) Console.WriteLine("{0} has evolved at {1} million years", name, timeConvertor); 

                timePrinted = false; 
            } 
        } 
    } 


    //START OF CLASSES 

    abstract class Star //Star is the abstract parent class that provides the outline that all star subclasses are based off 
                        //The Star class itself is never instantiated 
    { 
        protected double mass; 
        protected string name; 
        protected int[,] position = new int[1,3]; 
        protected double nextPhaseTime;
 
        public void setName(string inpName) 
        { 
            name = inpName; 
        } 

        public string getName() 
        { 
            return name; 
        } 

        public void setMass(double inpMass) 
        { 
            mass = inpMass; 
        } 

        public abstract double timeToNextPhase(); 
        public abstract void printEvolution(); 
    } 

    class Birth : Star 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a nebula", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 300; //300ms = 3 million years 
            return nextPhaseTime; 
        } 
    } 

    class Development : Star 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a protostar", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 50; //50ms = 0.5 million years 
            return nextPhaseTime; 
        } 
    } 

    abstract class Life : Star 
    { 
    } 

    abstract class Death1 : Star 
    { 
        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 0; 
            return nextPhaseTime; 
        } 
    } 

    abstract class Death2 : Star 
    { 
        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 0; 
            return nextPhaseTime; 
        } 
    } 

    class Death3 : Star 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a black dwarf", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 0; 
            return nextPhaseTime; 
        } 
    } 

    class BrownDwarf : Life 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a brown dwarf", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 1000; //10 million years 
            return nextPhaseTime; 
        } 
    } 

    class MainSequence : Life 
    { 
        private string spectralClass; 
        private string colour; 

        public void setSpectralClass() 
        { 
            if (mass > 18) 
            { 
                spectralClass = "O"; 
                colour = "blue"; 
                nextPhaseTime = 1000; //10 million years 
            } 
            else if (mass > 2.7) 
            { 
                spectralClass = "B"; 
                colour = "blue-white"; 
                nextPhaseTime = 10000; //100 million years 
            } 
            else if (mass > 1.5) 
            { 
                spectralClass = "A"; 
                colour = "white";
                //nextPhaseTime = 100000; //1 billion years 
            } 
            else if (mass > 1.1) 
            { 
                spectralClass = "F"; 
                colour = "yellow-white"; 
                //nextPhaseTime = 300000; //3 billion years 
            } 
            else if (mass > 0.8) 
            { 
                spectralClass = "G"; 
                colour = "yellow"; 
                //nextPhaseTime = 1000000; //10 billion years 
            } 
            else if (mass > 0.45) 
            { 
                spectralClass = "K"; 
                colour = "orange"; 
                //nextPhaseTime = 2500000; //25 billion years 
            } 
            else 
            { 
                spectralClass = "M"; 
                colour = "red"; 
                //nextPhaseTime = 10000000; //100 billion years 
            } 
        } 

        public string getSpectralClass() 
        { 
            return spectralClass; 
        } 

        public string getColour() 
        { 
            return colour; 
        } 

        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a main sequence star", name); 
        } 

        public override double timeToNextPhase() 
        { 
            return nextPhaseTime; 
        } 
    } 

    class BlueDwarf : Death1 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a blue dwarf", name); 
        } 

        public override double timeToNextPhase() 
        { 
            //nextPhaseTime = 100000; //1 billion years 
            return nextPhaseTime; 
        } 
    } 

    class RedGiant : Death1 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a red giant", name); 
        } 

        public override double timeToNextPhase() 
        { 
            //nextPhaseTime = 100000; //1 billion years 
            return nextPhaseTime; 
        } 
    } 

    class RedSupergiant : Death1 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a red supergiant", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 5000; //50 million years 
            return nextPhaseTime; 
        } 
    } 

    class WhiteDwarf : Death2 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a white dwarf", name); 
        } 

        public override double timeToNextPhase() 
        { 
            //nextPhaseTime = 1500000; //15 billion years 
            return nextPhaseTime; 
        } 
    } 

    class NeutronStar : Death2 
    { 
        public override void printEvolution() 
        { 
            Console.WriteLine("{0} is a neutron star", name); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 0; //Final stage of evolution 
            return nextPhaseTime; 
        } 
    } 

    class BlackHole : Death2 
    { 
        private double finalSchRadius; 
        private double[] schRadiusExMan = new double[2]; 
        private double[] gravConst = new double[2] { 6.674, -11 }; //Gravitational constant = 6.674 x 10^-11. Split into mantissa and exponent 
        int[] lightSpeedSquared = { 9, 16 }; //Lightspeed squared = 9 x 10^16. split into mantissa and exponent. 
        private double[] convertor = new double[2] { 1.989, 30 }; //Converts masses between solar masses (which they are input in) and kg (SI unit needed for calculation)x10^30 standard form 
        double[] siMass = new double[2]; 

        //Values to high or low powers of 10, such as 1.989 x 10^30, cannot be handled by the code 
        //In order to perform calculations with these numbers, I have split them into exponents and mantissae 

        public void setSchRadius() 
        { 
            siMass[0] = mass * convertor[0]; //converts star's mass from solar masses to the SI units needed for calculation 
            siMass[1] = siMass[1] + convertor[1]; 

            schRadiusExMan[0] = 2 * gravConst[0] * siMass[0] / lightSpeedSquared[0]; // (2G * mass) / c^2 
            schRadiusExMan[1] = gravConst[1] + siMass[1] - lightSpeedSquared[1]; 
            finalSchRadius = schRadiusExMan[0] * Math.Pow(10, schRadiusExMan[1]); //Multiplies exponent by 10 to the mantissa to give final answer 
        } 

        public void inSchRad(List<string> nameList, int[,] inpPosition) 
        { 
            List<int> starsInRad = new List<int>(); 
            bool inRadius = false; 

            for (int i = 0; i < nameList.Count - 1; i++) 
            { 
                int changeInX = inpPosition[i, 0] - inpPosition[nameList.Count - 1, 0]; 
                int changeInY = inpPosition[i, 1] - inpPosition[nameList.Count - 1, 1]; 
                int changeInZ = inpPosition[i, 2] - inpPosition[nameList.Count - 1, 2]; 
                int distance = Convert.ToInt32(Math.Sqrt(changeInX * changeInX + changeInY * changeInY + changeInZ * changeInZ)); 
                if (distance < finalSchRadius) 
                { 
                    starsInRad.Add(i); 
                    inRadius = true; 
                } 
            } 

            if (inRadius) 
            { 
                Console.WriteLine("Stars in range of the black hole: "); 
                for (int x = 0; x < starsInRad.Count; x++) 
                { 
                    Console.WriteLine(nameList[starsInRad[x]]); 
                } 
            } 
        } 

        public double getSchRadius() 
        { 
            return finalSchRadius; 
        } 

        public override void printEvolution() 

        { 
            Console.WriteLine("{0} is a black hole", name); 
        } 

        public void setPosition(int[,] inpPosition, int currentStar) 
        { 
            Console.WriteLine(currentStar); 
            position[0, 0] = inpPosition[currentStar, 0]; 
            position[0, 1] = inpPosition[currentStar, 1]; 
            position[0, 2] = inpPosition[currentStar, 2]; 
            Console.WriteLine("{0}, {1}, {2}", position[0,0], position[0,1], position[0,2]); 
        } 

        public override double timeToNextPhase() 
        { 
            nextPhaseTime = 0; 
            return nextPhaseTime; //Final stage of evolution 
        } 
    } 
} 