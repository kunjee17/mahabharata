let sample = "Ugrasrava, the son of Lomaharshana, surnamed Sauti"

let contain item (text:string) = text.Contains item
let in' x : string = x
let a x : string = x
let does x : string = x
let getIndexOf (item:char) (text:string) = text.IndexOf item


let parseComma (text:string) =
  match does text |> contain (a ",") with
  | true ->
      let index = getIndexOf ',' (in' text)
      let fstPart = text.Substring (0, index)
      let sndPart = text.Substring index
      fstPart, Some sndPart
  | false ->
      text, None

let parseSonOf (text:string) =
  match does text |> contain "the son of" with
  | true ->
      //let who, father = text
      let index = text.IndexOf("the son of")
      let fstPart = text.Substring(0, index)
      let sndPart = text.Substring index
      fstPart, Some sndPart
  | false ->
      text, None
      


let s = sample |> parseComma |> snd |> Option.bind (fun s -> s |> parseSonOf |> snd)



