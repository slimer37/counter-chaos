using System.Collections;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Interactables.Holding;
using Tutorial.Visuals;
using UnityEngine;

namespace Tutorial
{
    public class BasicTutorial : Tutorial
    {
        [SerializeField] Pickuppable itemToPickUp;
        [SerializeField] Transform goalGhostGroup;
        [SerializeField] Transform shelfGroup;
        [SerializeField] Transform itemGroup;
        [SerializeField] float okPosError;
        [SerializeField] Vector3 okRotError;

        readonly List<Transform> goals = new List<Transform>();
        readonly List<Pickuppable> items = new List<Pickuppable>();

        Collider[] shelfColliders;

        void Awake()
        {
            foreach (Transform child in goalGhostGroup) goals.Add(child);
            foreach (Transform child in itemGroup) items.Add(child.GetComponent<Pickuppable>());
            shelfColliders = shelfGroup.GetComponentsInChildren<Collider>();
        }

        bool ItemsArePlaced()
        {
            foreach (var goal in goals)
            {
                if (goal.localScale == Vector3.zero) continue;
                
                Pickuppable closest = null;
                var closestDist = 0f;
                foreach (var item in items)
                {
                    if (item.IsHeld) continue;
                    var dist = (goal.position - item.transform.position).sqrMagnitude;
                    if (!closest || dist < closestDist)
                    {
                        closest = item;
                        closestDist = dist;
                    }
                }

                if (!closest) break;
                
                if (closestDist < okPosError && IsWithinError(closest.transform.eulerAngles, goal.eulerAngles, okRotError))
                {
                    closest.GetComponent<Rigidbody>().isKinematic = true;
                    closest.transform.position = goal.position;
                    closest.transform.rotation = goal.rotation;
                    closest.gameObject.layer = 0;
                    goal.localScale = Vector3.zero;
                    items.Remove(closest);
                }
            }
            
            return items.Count == 0;

            bool IsWithinError(Vector3 rot1, Vector3 rot2, Vector3 maxError, float rotationalSymmetry = 90)
            {
                for (var i = 0; i < 3; i++)
                {
                    var value = rot1[i] % rotationalSymmetry - rot2[i] % rotationalSymmetry;
                    value = Mathf.Min(value, rotationalSymmetry - value);
                    if (value > maxError[i])
                        return false;
                }
                return true;
            }
        }

        void Start() => StartCoroutine(OnTutorialStart());
        
        public override IEnumerator OnTutorialStart()
        {
            var actions = Controls.Gameplay;
            var locator = LocatorHint.Instance;
            var textBox = TextBox.Instance;
            
            itemToPickUp.gameObject.SetActive(false);
            goalGhostGroup.gameObject.SetActive(false);
            itemGroup.gameObject.SetActive(false);

            yield return textBox.Display(true,
                "You can move with WASD.",
                $"You can also sprint with {actions.Sprint.FormatDisplayString()} and jump with {actions.Jump.FormatDisplayString()}.");
            yield return new WaitForSeconds(1);

            var interact = actions.Interact.FormatDisplayString();
            yield return textBox.Display(true, $"You can interact with {interact}.");
            
            yield return new WaitForSeconds(1);
            
            itemToPickUp.gameObject.SetActive(true);
            locator.ShowAndFollow(itemToPickUp.transform, $"Pick up ({interact})");
            yield return textBox.Display($"Find the highlighted object and pick it up with {interact}.");
            
            yield return new WaitUntil(() => itemToPickUp.IsHeld);
            locator.Hide();
            
            yield return new WaitForSeconds(1);
            textBox.Clear();
            
            yield return textBox.Display("The object was placed in your inventory.",
                "You can select different slots with the number keys or by using the mouse wheel. Try it now.");
            
            for (var i = 0; i < 2; i++)
            {
                var temp = Inventory.Main.ActiveSlotIndex;
                yield return new WaitUntil(() => Inventory.Main.ActiveSlotIndex != temp);
            }
            textBox.Clear();
            
            var drop = actions.Drop.FormatDisplayString();
            var toss = actions.Toss.FormatDisplayString();
            var rotate = actions.Rotate.FormatDisplayString();
            var slow = actions.Slow.FormatDisplayString();
            
            yield return textBox.Display(true, $"You can drop the item with {drop}.",
                $"Holding {drop} and looking at a surface will place the item on the surface.",
                "If you're aiming at a location that doesn't fit it, " +
                "a red ghost of the object will appear and the object will be placed in front of you.",
                $"While holding {drop}, you can also use {rotate} to rotate the item.");
            yield return new WaitForSeconds(1);
            yield return textBox.Display(true,
                $"For finer placement, you can hold {slow} to move slowly.");
            yield return new WaitForSeconds(1);
            yield return textBox.Display(true,
                $"You can also throw the item with {toss}.");
            yield return new WaitForSeconds(1);
            yield return textBox.Display(true,
                "If you try to drop or throw the object while it's inside a wall or other obstacle " +
                "(including yourself), it will return to your hand.");
            yield return new WaitForSeconds(1);
            
            yield return textBox.Display(
                "Practice some of these a little bit.\n" +
                $"Drop: {drop} | Rotate while dropping: {rotate} | Throw: {toss} | Slow down: {slow}");
            
            for (var i = 0; i < 5; i++)
            {
                yield return new WaitUntil(() => !itemToPickUp.IsHeld);
                yield return new WaitUntil(() => itemToPickUp.IsHeld);
            }
            
            textBox.Clear();
            yield return new WaitForSeconds(1);
            
            Inventory.Main.ClearAll();
            Destroy(itemToPickUp.gameObject);

            var destination = goalGhostGroup.position;
            goalGhostGroup.position += Vector3.up * 10;
            
            goalGhostGroup.gameObject.SetActive(true);
            itemGroup.gameObject.SetActive(true);

            foreach (var col in shelfColliders) col.enabled = false;

            shelfGroup.DOLocalMoveZ(0.5f, 1).SetRelative();
            shelfGroup.DOMoveY(10, 2).SetDelay(1);
            yield return goalGhostGroup.DOMove(destination, 1).WaitForCompletion();

            yield return textBox.Display(
                "A bunch of objects have spawned. Place them in the colored spaces. " +
                "Hint: If you're having trouble with the second-to-last cube, aim between the top two cubes of the tower.");
            yield return new WaitUntil(ItemsArePlaced);
            yield return new WaitForSeconds(1);
            
            textBox.Clear();
            yield return textBox.Display(
                "You're ready! " +
                $"Press {Controls.Menu.Exit.FormatDisplayString()} and head to the title screen.");
        }
    }
}
