namespace Hw7

module Tasks =
  let fib n = 
    let rec fib' acc1 acc2 = function
    | n when n > 1 -> fib' (acc1 + acc2) (acc1) (n - 1)
    | _            -> acc1
    in fib' 1 0 n
  
  let reverse list =
    let rec reverse' acc = function
    | x::xs -> reverse' (x::acc) xs
    | []    -> acc
    in reverse' [] list

  let rec mergesort = function
  | []  -> []
  | [x] -> [x]
  | l   ->
    let rec merge xs ys = match (xs, ys) with
    | ([], r) -> r
    | (l, []) -> l
    | (x::xt, y::_ ) when x <  y -> x::merge xt ys
    | (x::_ , y::yt) when x >= y -> y::merge xs yt
    in
    let (left, right) = List.splitAt (List.length l / 2) l in
    merge (mergesort left) (mergesort right)

  type Op =
  | Add
  | Sub
  | Mul
  | Div

  let applyOp = function
  | Add -> (+)
  | Sub -> (-)
  | Mul -> (*)
  | Div -> (/)

  type Expr =
  | Literal of int
  | Binary of Op * Expr * Expr

  let rec eval = function
  | Literal n -> n
  | Binary (op, l, r) -> applyOp op (eval l) (eval r)

  let primes: int list =
    let rec seive (s: int seq): int list =
      if Seq.isEmpty s
        then []
        else let p  = Seq.head s in
             let ps = seive ( Seq.filter (fun n -> n % p = 0) ( Seq.tail s )) in
             p::ps
    in seive <| Seq.initInfinite id