(**
Lesson 1 - Basic understanding of NLP. Getting started with term,document frequency for term and Inverse document frequency for term. 
*)

open System.IO
open System
open System.Text.RegularExpressions


type Book(bookno:string) = 

    //should be done with regex. But I don't know regex so I copy pasted from SO to remove all special chars
    let removeSpecialChars (str : string) = Regex.Replace(str, "[^0-9a-zA-Z]+", " ")

    member x.BookText = 
                Path.Combine(__SOURCE_DIRECTORY__, "..", "books/"+ bookno + ".txt")
                |> File.ReadAllLines

    member x.BookTermGroup = 
        x.BookText 
            |> String.concat " " 
            |> (fun x -> x.Split ' ')
            |> Array.map (removeSpecialChars >> (fun x -> x.Trim())) 
            |> Array.filter (fun x -> x <> "")
            |> Array.groupBy id 
            |> Array.map (fun (a,b) -> a, b.Length )

    member x.BookUniqueTerms = 
        x.BookTermGroup 
            |> Array.map (fun (x,_)-> x)
            |> set


let booknos = [|"01";"02";"03";"04";"05";"06";"07";"08";"09";"10";"11";"12";"13";"14";"15";"16";"17";"18"|]

let books = [|
    for n in booknos do
        yield new Book(n) 
|]

let allUniqueTermsbut1 = books.[1..] |> Array.map (fun x -> x.BookUniqueTerms) |> Array.fold (fun acc elem -> acc + elem) Set.empty

let termsspecificto1 = books.[0] |> fun x -> x.BookUniqueTerms - allUniqueTermsbut1



termsspecificto1 |> Set.toArray |> Array.take 50 |> Array.iter (printfn "%A")


