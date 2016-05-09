#load "NameFinder.fs"
#I "../packages/fparsec/lib/net40-client"
#r "FParsec.dll"
#r "FParsecCS.dll"

open FParsec

let txt = """On hearing this, Yudhishthira asked, 'O great Muni, whose sons were
Asuras called Sunda and Upasunda? Whence arose that dissension amongst
them, and why did they slay each other? Whose daughter also was this
Tilottama for whose love the maddened brothers killed each other? Was she
an Apsara (water nymph) or the daughter of any celestial? O thou whose
wealth is asceticism, we desire, O Brahmana, to hear in detail everything
as it happened. Indeed, our curiosity hath become great.'"""


let clearStr = NameFinder.removeSpecialChars txt + "@"


let parseAllExceptQuote = charsTillString "@" false 1000
let parseQuotedString = skipCharsTillString "asked" true 1000 >>. parseAllExceptQuote
let parsedQuote = run parseQuotedString clearStr

let quote p = 
    match p with
    | Success(q, _, _) -> q
    | Failure(_, _, _) -> "Could not parse string"

let words = (quote parsedQuote) |> NameFinder.names

let matches words= 
    [| for word in words do
          if NameFinder.namesOnly |> Array.contains word then yield word |]

let actorDialog sen = 
    let clearStr = NameFinder.removeSpecialChars sen + "@" //adding @ to find the end of string as all special char removed
    
    let actor = 
        run (charsTillString "asked" false 1000) clearStr |> fun p -> 
            match p with
            | Success(q, _, _) -> q
            | Failure(_) -> failwith "can't parse string"
            |> NameFinder.names |> matches |> Array.tryHead |> fun x -> x.Value
    let dialog = 
        run (skipCharsTillString "asked" true 1000 >>. charsTillString "@" false 1000) clearStr |> fun p -> 
            match p with
            | Success(q, _, _) -> q
            | Failure(_) -> failwith "can't parse string"
            |> NameFinder.names |> matches |> Array.countBy id
    (actor, dialog)

actorDialog txt