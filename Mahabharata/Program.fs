open System
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open System.IO

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.


(* 
    Filtering name based on capital later. Thanks to Eve. 
*)
let checkisName (str:string) = Char.IsUpper (str.ToCharArray().[0])
//should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
let cleanedUpSample (str:string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ");
//remove roman numbers 
let isRoman str = Regex.IsMatch(str, "^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")
let filterObvious str = 
    str <> "I" && str <> "A" && str <> "An" && str <> "The"
let allProbableNames (str:string)=  str.Split(' ') 
                                    |> Array.toSeq
                                    |> PSeq.map (fun x -> x.Trim())
                                    |> PSeq.filter (fun x -> not <| (String.IsNullOrWhiteSpace x))
                                    |> PSeq.filter checkisName
                                    |> PSeq.filter filterObvious //filter the obvious
                                    |> PSeq.filter (fun x -> (isRoman >> not) x)
                                    |> PSeq.filter (fun x -> not <| str.Contains(x.ToLower()))
let names = cleanedUpSample >> allProbableNames //this would have all name appearance in sample. 
let uniqueNames str= names str |> PSeq.distinct

//big volume1 data. 
let volume1path = Path.Combine(__SOURCE_DIRECTORY__,"..","txt_data/volume1.txt")
let volume1 = File.ReadAllText volume1path


[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    uniqueNames volume1 |> Seq.take 100 |> Seq.iter (fun x -> printfn "%A" x)
    System.Console.Read() |> ignore
    0 // return an integer exit code
