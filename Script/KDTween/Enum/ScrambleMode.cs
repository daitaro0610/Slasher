namespace KD.Tweening
{
    public enum ScrambleMode
    {
        None,       //スクランブルしない
        All,        //A-Z、a-z、0-9のすべてを使ってスクランブルする
        Uppercase,  //A-Zを使ってスクランブルする
        Lowercase,  //a-zを使ってスクランブルする
        Numerals,   //0-9を使ってスクランブルをする
        Custom      //自身が設定したCharでスクランブルする
    }
}