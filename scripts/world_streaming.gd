class_name RPWorldStreaming
extends Node

@export var cell_size := 80.0
@export var active_radius := 2
var streamed_nodes: Dictionary = {}

func register_chunk(chunk_id: Vector2i, node: Node3D) -> void:
    streamed_nodes[chunk_id] = node

func update_player_position(position: Vector3) -> void:
    var center := Vector2i(floor(position.x / cell_size), floor(position.z / cell_size))
    for id in streamed_nodes:
        var node: Node3D = streamed_nodes[id]
        var active := abs(id.x - center.x) <= active_radius and abs(id.y - center.y) <= active_radius
        if node.visible != active:
            node.visible = active
