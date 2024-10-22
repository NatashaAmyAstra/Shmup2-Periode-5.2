using UnityEngine;
using System.Collections;

public class BossSpiralPhase : BossPhase
{
    public BossSpiralPhase(Vector3 startPosition) : base(startPosition) { }

    public override void EnterPhase(FinalBoss boss) {
        StartCoroutine(boss, ShootPattern);
        StartCoroutine(boss, MovePattern);
    }

    protected override IEnumerator ShootPattern(FinalBoss boss) {
        while(true)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MovePattern(FinalBoss boss) {
        Vector3 startPosition = boss.transform.position;
        float t = 0;
        
        // move in a spiral motion
        do
        {
            t += 1 / boss.CycleDurationSeconds * Time.deltaTime;

            MoveCircle(boss, startPosition, t);
            yield return new WaitForEndOfFrame();
        } while(t <= 1);

        ExitPhase(boss);
    }

    private void MoveCircle(FinalBoss boss, Vector3 startPosition, float t) {
        // radius is on a sin wave to increase and decrease over time
        float radius = Mathf.Sin(t * Mathf.PI) * boss.SpiralMaxRadius;

        // 0-1 is a full circle. t, being a 0-1 value by nature, is multiplied to get more circles
        float circlePosition = t * boss.NumberOfRotations;

        Vector3 position = Vector3.zero;
        position.x = Mathf.Cos(circlePosition * 2 * Mathf.PI) * radius;
        position.y = Mathf.Sin(circlePosition * 2 * Mathf.PI) * radius;

        boss.transform.position = startPosition + position;
    }
}
