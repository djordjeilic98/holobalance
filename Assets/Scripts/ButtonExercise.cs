using UnityEngine;
using System.Collections;

public class ButtonExercise : MonoBehaviour
{
    public enum ExerciseType
    {
        sitting,
        standing,
        walking
    }

    public enum ExerciseProgressions
    {
        P0, P1, P2, P3
    }

    public enum ExerciseNumber
    {
        ex1 = 1,
        ex2, ex3, ex4
    }

    public ExerciseType exType = ExerciseType.sitting;
    public ExerciseProgressions exProgression = ExerciseProgressions.P0;
    public ExerciseNumber exNumber = ExerciseNumber.ex1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
