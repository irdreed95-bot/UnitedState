"""Run inside the Unreal Editor Python console to bootstrap the CSRP world.

This script creates/loads the CSRP_Main level, enables the Open World/World Partition
workflow where supported by the installed UE5 version, and places named district
markers and Citizen Program interaction markers from the C++ layout registry.

It intentionally does not import third-party meshes automatically: asset licenses and
the PDF world layout must be reviewed before production assets are placed.
"""

import unreal

MAP_PACKAGE = "/Game/Maps/CSRP_Main"


def log(message):
    unreal.log("[CSRP World] " + message)


def ensure_directory():
    if not unreal.EditorAssetLibrary.does_directory_exist("/Game/Maps"):
        unreal.EditorAssetLibrary.make_directory("/Game/Maps")


def create_or_load_map():
    ensure_directory()

    if unreal.EditorAssetLibrary.does_asset_exist(MAP_PACKAGE):
        unreal.EditorLevelLibrary.load_level(MAP_PACKAGE)
        log("Loaded existing CSRP_Main map.")
        return

    # New level creation is editor-version dependent. The Open World template
    # should be preferred when available; this fallback creates an empty level.
    try:
        world = unreal.EditorLevelLibrary.new_level(MAP_PACKAGE)
        if world:
            unreal.EditorLevelLibrary.save_current_level()
            log("Created empty CSRP_Main map.")
    except Exception as exc:
        log("Automatic map creation is unavailable in this UE version: {}".format(exc))
        log("Create an Open World level named CSRP_Main at /Game/Maps, then rerun this script.")


def place_bootstrap():
    world = unreal.EditorLevelLibrary.get_editor_world()
    if not world:
        return

    actor_subsystem = unreal.get_editor_subsystem(unreal.EditorActorSubsystem)
    existing = actor_subsystem.get_all_level_actors()

    for actor in existing:
        if actor.get_class().get_name() == "CSRPWorldBootstrap":
            log("CSRPWorldBootstrap already exists; skipping duplicate.")
            return

    bootstrap_class = unreal.load_class(None, "/Script/CorruptStateRP.CSRPWorldBootstrap")
    if not bootstrap_class:
        log("C++ module is not loaded yet. Compile CorruptStateRP in UE5, then rerun.")
        return

    actor = actor_subsystem.spawn_actor_from_class(
        bootstrap_class,
        unreal.Vector(0.0, 0.0, 0.0),
        unreal.Rotator(0.0, 0.0, 0.0)
    )

    if actor:
        actor.set_actor_label("CSRP World Bootstrap")
        actor_subsystem.set_actor_selection_state(actor, True)
        unreal.EditorLevelLibrary.save_current_level()
        log("Placed CSRP World Bootstrap.")


def main():
    log("Starting world bootstrap...")
    create_or_load_map()
    place_bootstrap()
    log("World bootstrap finished.")


main()
