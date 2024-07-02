using System;
using System.Collections;
using System.Collections.Generic;

public interface ITrackManager
{
    TrackSegment currentSegment { get; }
    float currentSegmentDistance { get; }
    ThemeData currentTheme { get; }
    int currentZone { get; }
    bool firstObstacle { get; set; }
    bool isLoaded { get; set; }
    bool isMoving { get; }
    bool isRerun { get; set; }
    bool isTutorial { get; set; }
    int multiplier { get; }
    int score { get; }
    List<TrackSegment> segments { get; }
    float speed { get; }
    float speedRatio { get; }
    float timeToStart { get; }
    int trackSeed { get; set; }
    float worldDistance { get; }
    CharacterInputController CharacterController { get; }
    public delegate int MultiplierModifier(int current);
    public MultiplierModifier modifyMultiply { get; set; }
    Action<TrackSegment> newSegmentCreated { get; set; }
    Action<TrackSegment> currentSegementChanged { get; set; }
    float LaneOffset { get; }

    void AddScore(int amount);
    IEnumerator Begin();
    void ChangeZone();
    void End();
    void PowerupSpawnUpdate();
    IEnumerator SpawnCoinAndPowerup(TrackSegment segment);
    IEnumerator SpawnNewSegment();
    void SpawnObstacle(TrackSegment segment);
    void StartMove(bool isRestart = true);
    void StopMove();
}