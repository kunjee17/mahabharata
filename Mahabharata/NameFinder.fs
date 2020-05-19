//Parse name which comes before said
// open VerbalExpression
//can't make it work with string if I am passing "a said b" then it should return (a,b)
//fall back to string split. 
//TODO use regex or FParsec for better performace
module NameFinder
  open System.Text.RegularExpressions
  open System
  open System.IO
//  open FSharp.Collections.ParallelSeq

  



  (* 
      Filtering name based on capital later. Thanks to Eve. 
  *)
  let checkisName (str : string) = Char.IsUpper str.[0]
  //should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
  let removeSpecialChars (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")
  //remove roman numbers 
  let isRoman str = Regex.IsMatch(str, "^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")
  let filterObvious str = str <> "I" && str <> "A" && str <> "An" && str <> "The"
  let filterSalutation str = str <> "O"
  let combineFilter str = 
      (not <| String.IsNullOrWhiteSpace str) && (not <| isRoman str) && (filterObvious str) && (checkisName str)
      && filterSalutation str
   
  let processStr (str : string) = 
      str.Split(' ')
      |> Array.map (fun x -> x.Trim())

  let allProbableNames (str : string) =
      // Looking up lower case words was very slow 
      // because it was searching through the full text
      // Changed it into a Set - constant lookup
      let processed = processStr str |> set 
      processStr str
      |> Array.filter combineFilter
      |> Array.filter (fun x -> not (processed.Contains(x.ToLower())))

  let names = removeSpecialChars >> allProbableNames //this would have all name appearance in sample. 
  let uniqueNames str = names str |> Array.countBy id

  //big volume1 data. 
  let volume1 = 
      Path.Combine(__SOURCE_DIRECTORY__, "..", "txt_data/volume1.txt")
      |> File.ReadAllLines
      |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
      |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
      |> String.concat "\n"
        
  let volume2 = 
      Path.Combine(__SOURCE_DIRECTORY__, "..", "txt_data/volume2.txt")
      |> File.ReadAllLines
      |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
      |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
      |> String.concat "\n"
 
  let volume3 = 
      Path.Combine(__SOURCE_DIRECTORY__, "..", "txt_data/volume3.txt")
      |> File.ReadAllLines
      |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
      |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
      |> String.concat "\n"
        
  let volume4 = 
      Path.Combine(__SOURCE_DIRECTORY__, "..", "txt_data/volume4.txt")
      |> File.ReadAllLines
      |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
      |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
      |> String.concat "\n"
   
  //Book Nos as per filnames
  let booknos = [|
                    "01";
                    "02";
                    "03";
                    "04";
                    "05";
                    "06";
                    "07";
                    "08";
                    "09";
                    "10";
                    "11";
                    "12";
                    "13";
                    "14";
                    "15";
                    "16";
                    "17";
                    "18"
                    |]

  let volume =   
    booknos |>
     Array.map (fun n -> 
         Path.Combine(__SOURCE_DIRECTORY__, "..", "books/"+ n + ".txt")
          |> File.ReadAllLines
          |> Array.skipWhile (fun line -> line <> "THE MAHABHARATA") // Skip the description & preface
          |> Array.takeWhile (fun line -> line <> "FOOTNOTES")    // Skip the footnotes
          |> String.concat "\n"
        )
        |> Array.fold ( + ) ""
//   let volume = volume1 + volume2 + volume3 + volume4   
        

  let printSeq strs = strs |> Array.iter (fun x -> printfn "%A" x)

//   printSeq <| (uniqueNames volume1 |> Array.take 100)
  let nameCandidates = uniqueNames volume

  // Look at top names
  let nameCandidatesArr =
    nameCandidates
    |> Array.sortByDescending snd

  let namesOnly = nameCandidatesArr |> Array.map fst

  let top50 =
    nameCandidatesArr
    |> Array.take 50

  top50
  |> printSeq

