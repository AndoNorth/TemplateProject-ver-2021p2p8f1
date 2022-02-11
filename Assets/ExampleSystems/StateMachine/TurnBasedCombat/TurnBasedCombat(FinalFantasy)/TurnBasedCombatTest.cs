using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TemplateProject {
    namespace TurnBasedCombatFinalFantasy {
        public class TurnBasedCombatTest : MonoBehaviour
        {
            [SerializeField]
            Sprite[] _characterSprites;
            public Sprite[] CharacterSprites { get { return _characterSprites; } }
            StateMachine _stateMachine;
            Character[] _characters = new Character[6];
            public Character[] Characters { get { return _characters; } }
            Dictionary<Character, Transform> _charToSlots = new Dictionary<Character, Transform>();
            List<Character> _charsCanAttack = new List<Character>();
            bool _setupComplete = false;
            public bool SetupBool { get { return _setupComplete; } }
            bool _reset;
            public bool ResetBool { get { return _reset; } }
            private void Awake()
            {
                Reset();
            }
            [SerializeField]
            private bool _debugStateText = false;
            public bool DebugStateText { get { return _debugStateText; } }
            [SerializeField]
            private float tickRate = 1f;
            void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 position = GeneralUtility.GetMouseWorldPosition();
                    InvokeRepeating("Tick", 0.5f, tickRate);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    CancelInvoke();
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    _stateMachine.SetState(new StartState(this));
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    _stateMachine.SetState(new AssessBattle(this));
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    _stateMachine.SetState(new ResolveRound(this));
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    _stateMachine.SetState(new End(this));
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    SetSetupComplete(true);
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    SetSetupComplete(false);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _reset = true;
                    TextPopup.Create(Vector3.zero, "Reset", 16, Vector3.zero, TextPopup.TextPopupEffect.POP, 1f, 1f);
                }
                if (DebugStateText)
                {
                    string func1 = "IsSetupComplete:" + _setupComplete;
                    string func2 = "OneTeamDied:" + !(AnyBotsAlive() && AnyPlayersAive());
                    string func3 = "NoAttackersLeft:" + (_charsCanAttack.Count() <= 0);
                    string func4 = "AttackersLeft:" + (_charsCanAttack.Count() > 0);
                    string func5 = "IsResetSet:" + (ResetBool);
                    TextPopup.Create(new Vector3(0, -1.1f), func1, 6, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                    TextPopup.Create(new Vector3(0, -1.6f), func2, 6, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                    TextPopup.Create(new Vector3(0, -2.1f), func3, 6, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                    TextPopup.Create(new Vector3(0, -2.6f), func4, 6, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                    TextPopup.Create(new Vector3(0, -3.1f), func5, 6, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            void Tick()
            {
                _stateMachine.Tick();
                if (DebugStateText)
                {
                    TextPopup.Create(Vector3.zero, "Tick", 16, Vector3.zero, TextPopup.TextPopupEffect.POP, 1f, 1f);
                }
            }
            void SetupStateMachine()
            {
                // shortcut to add transition
                void At(IState from, IState to, Func<bool> condition) => _stateMachine.addTransition(from, to, condition);
                // initialise states
                StartState start = new StartState(this);
                ResolveRound resolveRound = new ResolveRound(this);
                AssessBattle assessBattle = new AssessBattle(this);
                End end = new End(this);
                ResetState reset = new ResetState(this);
                // initialise state machine
                _stateMachine = new StateMachine();
                // state transitions
                At(start, assessBattle, IsSetupComplete());
                At(resolveRound, assessBattle, NoAttackersLeft());
                At(assessBattle, resolveRound, AttackersLeft());
                _stateMachine.addAnyTransition(reset, IsResetSet()); // note: reset transition must be set first, as a priority system
                _stateMachine.addAnyTransition(start, IsSetupNotComplete());
                _stateMachine.addAnyTransition(end, OneTeamDied());
                // func bool methods
                Func<bool> IsSetupComplete() => () => SetupBool;
                Func<bool> IsSetupNotComplete() => () => !SetupBool;
                Func<bool> OneTeamDied() => () => !(AnyBotsAlive() && AnyPlayersAive());
                Func<bool> NoAttackersLeft() => () => _charsCanAttack.Count() <= 0;
                Func<bool> AttackersLeft() => () => _charsCanAttack.Count() > 0;
                Func<bool> IsResetSet() => () => ResetBool;
            }
            public void Reset()
            {
                _reset = false;
                SetSetupComplete(false);
                SetCharacters(new Character[6]);
                ResetCharToSlotsDictionary();
                SetupStateMachine();
            }
            public void SetCharacters(Character[] characters)
            {
                this._characters = characters;
            }
            public void SetSetupComplete(bool boolean)
            {
                _setupComplete = boolean;
            }
            public void ResetCharToSlotsDictionary()
            {
                _charToSlots = new Dictionary<Character, Transform>();
            }
            public void AddToCharsToSlots(Character character, Transform slot)
            {
                _charToSlots.Add(character, slot);
            }
            public Transform GetSlot(Character character)
            {
                return _charToSlots[character];
            }
            public void ClearCharsCanAttack()
            {
                _charsCanAttack.Clear();
            }
            public void AddToCharsCanAttack(Character character)
            {
                _charsCanAttack.Add(character);
            }
            public void RemoveFromCharsCanAttack(Character character)
            {
                _charsCanAttack.Remove(character);
            }
            public Character PopCharAt(int index)
            {
                if(_charsCanAttack.Count <= 0)
                {
                    return null;
                }
                Character character = _charsCanAttack[index];
                _charsCanAttack.RemoveAt(index);
                return character;
            }
            public bool AnyPlayersAive()
            {
                if (!SetupBool)
                {
                    return true;
                }
                return !_characters[0].IsDead || !_characters[1].IsDead || !_characters[2].IsDead;
            }
            public bool AnyBotsAlive()
            {
                if (!SetupBool)
                {
                    return true;
                }
                return !_characters[3].IsDead || !_characters[4].IsDead || !_characters[5].IsDead;
            }
            public bool AnyAlive()
            {
                return AnyPlayersAive() || AnyBotsAlive();
            }
            public Character[] AliveCharacters()
            {
                if (!AnyAlive())
                {
                    return null;
                }
                List<Character> aliveCharacters = new List<Character>();
                foreach (Character character in Characters)
                {
                    if (!character.IsDead)
                    {
                        aliveCharacters.Add(character);
                    }
                }
                return aliveCharacters.ToArray();
            }
            public bool IsPlayer(Character character)
            {
                return character.Team == Character.CharacterTeams.Player;
            }
            public bool IsBot(Character character)
            {
                return character.Team == Character.CharacterTeams.Bot;
            }
            public void DrawRayShot(Vector3 _shotOrigin, Vector3 _rayHitPoint)
            {
                TrailRenderer trail = Instantiate(GameAssets.instance.ShotTrail, _shotOrigin, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, _rayHitPoint));
            }
            private IEnumerator SpawnTrail(TrailRenderer _trail, Vector3 _rayHitPoint)
            {
                float time = 0;
                float trailTravelTime = 0.8f;
                Vector3 startPosition = _trail.transform.position;

                while (time < trailTravelTime)
                {
                    _trail.transform.position = Vector3.Lerp(startPosition, _rayHitPoint, time);
                    time += Time.deltaTime / _trail.time;

                    yield return null;
                }
                _trail.transform.position = _rayHitPoint;

                Destroy(_trail.gameObject, _trail.time);
            }
            public void ChangeSpriteColorForTime(SpriteRenderer sprite, Color newColor, Color oldColor, float time)
            {
                StartCoroutine(ChangeSpriteColorForTimeRoutine(sprite, newColor, oldColor, time));
            }
            private IEnumerator ChangeSpriteColorForTimeRoutine(SpriteRenderer sprite,Color newColor, Color oldColor, float time)
            {
                sprite.color = newColor;
                yield return new WaitForSeconds(time);
                sprite.color = oldColor;
            }
        }
        #region States
        class TemplateState : IState {
            string stateName = "Template State";
            public TemplateState()
            {

            }
            public void Tick()
            {

            }
            public void OnEnter()
            {
                TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
            public void OnExit()
            {
                TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
            }
        }
        class StartState : IState
        {
            TurnBasedCombatTest turnBasedCombatTest;
            string stateName = "Start";
            bool charactersSetup = false;
            bool slotsSetup = false;
            bool spritesSetup = false;
            Character[] characters;
            public StartState(TurnBasedCombatTest turnBasedCombatTest)
            {
                this.turnBasedCombatTest = turnBasedCombatTest;
            }
            public void Tick()
            {
                if (charactersSetup && slotsSetup && spritesSetup)
                {
                    turnBasedCombatTest.SetSetupComplete(true);
                }
                else
                {
                    SetupCharacters();
                    SetupSlots();
                    SetupSprites(3);
                }
            }
            public void OnEnter()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            public void OnExit()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, -4), "Exited State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            // setup characters
            private void SetupCharacters()
            {
                if (!charactersSetup)
                {
                    characters = new Character[6] {
                    new Character(10, 2, Character.CharacterTypes.Square, Character.CharacterTeams.Player) ,
                    new Character(10, 2, Character.CharacterTypes.Square, Character.CharacterTeams.Player) ,
                    new Character(10, 2, Character.CharacterTypes.Square, Character.CharacterTeams.Player) ,
                    new Character(10, 2, Character.CharacterTypes.Circle, Character.CharacterTeams.Bot) ,
                    new Character(10, 2, Character.CharacterTypes.Circle, Character.CharacterTeams.Bot) ,
                    new Character(10, 2, Character.CharacterTypes.Circle, Character.CharacterTeams.Bot) };
                    turnBasedCombatTest.SetCharacters(characters);
                    charactersSetup = true;
                }
            }
            private void SetupSlots()
            {
                if (!slotsSetup)
                {
                    int slotIdx = 0;
                    foreach (Transform side in turnBasedCombatTest.transform) // iterates over children
                    {
                        foreach (Transform slot in side.transform) // iterates over children
                        {
                            Character character = characters[slotIdx];
                            turnBasedCombatTest.AddToCharsToSlots(character, slot);
                            slotIdx++;
                        }
                    }
                    slotsSetup = true;
                }
            }
            private void SetupSprites(int method)
            {
                if (!spritesSetup)
                {
                    int slotIdx = 0;
                    if (method == 1)
                    {
                        foreach (Transform side in turnBasedCombatTest.transform) // iterates over children
                        {
                            foreach (Transform slot in side.transform) // iterates over children
                            {
                                SpriteRenderer slotSR = slot.GetComponent<SpriteRenderer>();
                                Character character = characters[slotIdx];
                                slotSR.sprite = turnBasedCombatTest.CharacterSprites[(int)character.CharacterType];
                                slotSR.color = character.Color;
                                character.SetPosition(slotSR.gameObject.transform.position);
                                character.SetHealthBar(new HealthBar(character.Position, slot.name, slot));
                                character.SetName(slot.name);
                                slotIdx++;
                            }
                        }
                    }
                    else if (method == 2)
                    {
                        /*
                        foreach (SpriteRenderer slot in FindObjectsOfType<SpriteRenderer>())
                        {
                            slot.sprite = turnBasedCombatTest.CharacterSprites[(int)characters[slotIdx].CharacterType];
                            slotIdx++;
                        }
                        */
                    }
                    else if (method == 3)
                    {
                        foreach (Character character in turnBasedCombatTest.Characters)
                        {
                            Transform slot = turnBasedCombatTest.GetSlot(character);
                            SpriteRenderer slotSR = slot.GetComponent<SpriteRenderer>();
                            slotSR.sprite = turnBasedCombatTest.CharacterSprites[(int)character.CharacterType];
                            slotSR.color = (character.Team == Character.CharacterTeams.Player) ? Color.magenta : Color.blue;
                            character.SetPosition(slotSR.gameObject.transform.position);
                            character.SetHealthBar(new HealthBar(character.Position, slot.name, slot));
                            character.SetName(slot.name);
                        }
                    }
                    spritesSetup = true;
                }
            }
        }
        class ResolveRound : IState
        {
            TurnBasedCombatTest turnBasedCombatTest;
            string stateName = "Resolve Round";
            public ResolveRound(TurnBasedCombatTest turnBasedCombatTest)
            {
                this.turnBasedCombatTest = turnBasedCombatTest;
            }
            public void Tick()
            {
                Character nextAttacker = FindNextAttacker();
                if (nextAttacker == null)
                {
                    return;
                }
                ShootRandomEnemy(nextAttacker);
            }
            public void OnEnter()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            public void OnExit()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, -4), "Exited State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            Character FindNextAttacker()
            {
                return turnBasedCombatTest.PopCharAt(0);
            }
            void ShootRandomEnemy(Character character)
            {
                Character[] targetCharacters = turnBasedCombatTest.IsPlayer(character) ? turnBasedCombatTest.Characters[3..6] : turnBasedCombatTest.Characters[0..3];
                Character target = FindTarget(character, targetCharacters);
                bool targetDied = character.DealDamage(target);
                turnBasedCombatTest.DrawRayShot(character.Position, target.Position);
                turnBasedCombatTest.ChangeSpriteColorForTime(turnBasedCombatTest.GetSlot(target).GetComponent<SpriteRenderer>(), Color.white, target.Color, 0.2f) ;
                TextPopup.Create(target.Position, character.Damage, 6, UnityEngine.Random.insideUnitCircle, TextPopup.TextPopupEffect.POP, 10f, 0.5f);
                if (targetDied)
                {
                    turnBasedCombatTest.RemoveFromCharsCanAttack(target);
                }
            }
            Character FindTarget(Character seeker, Character[] targetCharacters)
            {
                bool findingTarget = true;
                Character target;
                while (findingTarget)
                {
                    int randVal = UnityEngine.Random.Range(0, 3);
                    target = targetCharacters[randVal];
                    if (!target.IsDead)
                    {
                        return target;
                    }
                }
                return null;
            }
        }
        class AssessBattle : IState
        {
            TurnBasedCombatTest turnBasedCombatTest;
            string stateName = "Assess Battle";
            public AssessBattle(TurnBasedCombatTest turnBasedCombatTest)
            {
                this.turnBasedCombatTest = turnBasedCombatTest;
            }
            public void Tick()
            {

            }
            public void OnEnter()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
                turnBasedCombatTest.ClearCharsCanAttack();
                foreach (Character character in turnBasedCombatTest.AliveCharacters())
                {
                    turnBasedCombatTest.AddToCharsCanAttack(character);
                }
            }
            public void OnExit()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, -4), "Exited State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
        }
        class End : IState
        {
            TurnBasedCombatTest turnBasedCombatTest;
            string stateName = "End";
            public End(TurnBasedCombatTest turnBasedCombatTest)
            {
                this.turnBasedCombatTest = turnBasedCombatTest;
            }
            public void Tick()
            {

            }
            public void OnEnter()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
                string finalText = "";
                if (turnBasedCombatTest.AnyPlayersAive())
                {
                    finalText = "Players Win";
                }
                else if (turnBasedCombatTest.AnyBotsAlive())
                {
                    finalText = "Bots Win";
                }
                TextPopup.Create(Vector3.zero, finalText, 24, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 3f);
            }
            public void OnExit()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, -4), "Exited State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
        }
        class ResetState : IState
        {
            TurnBasedCombatTest turnBasedCombatTest;
            string stateName = "Reset";
            public ResetState(TurnBasedCombatTest turnBasedCombatTest)
            {
                this.turnBasedCombatTest = turnBasedCombatTest;
            }
            public void Tick()
            {
                Reset();
            }
            public void OnEnter()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, 4), "Entered State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            public void OnExit()
            {
                if (turnBasedCombatTest.DebugStateText)
                {
                    TextPopup.Create(new Vector3(0, -4), "Exited State: " + stateName, 12, Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
                }
            }
            void Reset()
            {
                turnBasedCombatTest.Reset();
            }
        }
        #endregion
        public class HealthBar
        {
            GameObject _gameObject;
            private GeneralUtility.World_Bar _healthBar;

            public HealthBar(Vector3 position, string characterName, Transform parent)
            {
                _gameObject = new GameObject(characterName + "'s Health Bar");
                _gameObject.transform.position = position;
                _gameObject.transform.parent = parent;
                GeneralUtility.World_Bar.Outline outline = new GeneralUtility.World_Bar.Outline();
                outline.size = 0.1f;

                _healthBar = new GeneralUtility.World_Bar(_gameObject.transform, new Vector3(0, 1.2f), new Vector3(2f, 0.2f), Color.white, Color.red, 1f, -10, outline);
            }
            public void SetHealthBar(float fillAmount)
            {
                _healthBar.SetSize(fillAmount);
            }
        }
        public class Character
        {
            public enum CharacterTypes
            {
                Square = 0,
                Circle,
                Hexagon
            }
            public enum CharacterTeams
            {
                Player = 0,
                Bot
            }
            int _maxHealth;
            int _currentHealth;
            public int CurrentHealth { get { return _currentHealth; } }
            int _damage;
            public int Damage { get { return _damage; } }
            CharacterTypes _characterType;
            public CharacterTypes CharacterType { get { return _characterType; } }
            CharacterTeams _team;
            public CharacterTeams Team { get { return _team; } }
            public bool IsDead { get { return _currentHealth <= 0; } }
            private Vector3 _position;
            public Vector3 Position { get { return _position; } }
            private HealthBar _healthBar;
            private string _name;
            public string Name { get { return _name; } }
            public Color Color { get { return (Team == Character.CharacterTeams.Player) ? Color.magenta : Color.blue; } }
            public Character(int maxHealth, int damage, CharacterTypes characterType, CharacterTeams team)
            {
                this._maxHealth = maxHealth;
                this._damage = damage;
                this._characterType = characterType;
                this._team = team;
                SetHealth(maxHealth);
            }
            private void SetHealth(int health)
            {
                this._currentHealth = health;
            }
            public void SetPosition(Vector3 position)
            {
                this._position = position;
            }
            public void SetHealthBar(HealthBar healthBar)
            {
                healthBar.SetHealthBar((float)_currentHealth / (float)_maxHealth);
                this._healthBar = healthBar;
            }
            public void SetName(string name)
            {
                this._name = name;
            }
            public bool TakeDamage(Character from, int damage)
            {
                _currentHealth -= damage;
                bool died = false;
                if (IsDead)
                {
                    SetHealth(0);
                    TextPopup.Create(_position, "Died", 10, new Vector3(0, 1), TextPopup.TextPopupEffect.FLOAT, 5f);
                    died = true;
                }
                _healthBar.SetHealthBar((float) _currentHealth / (float) _maxHealth);
                return died;
            }
            public bool DealDamage(Character target)
            {
                return target.TakeDamage(this, _damage);
            }
        }
    }
}
