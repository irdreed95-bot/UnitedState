class_name RPInteractable
extends Node3D

@export var interaction_name := "تفاعل"
@export_multiline var interaction_description := ""
var enabled := true

func can_interact() -> bool:
    return enabled

func interact(actor: Node) -> Dictionary:
    return {
        "name": interaction_name,
        "description": interaction_description
    }
