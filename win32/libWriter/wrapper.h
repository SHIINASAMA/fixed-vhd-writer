#include "writer.h"

// 小于 0 失败， 大于零实际写入字节数
extern "C" __declspec(dllexport) int64_t write(const char* vhd_name, int64_t lba, const char* data_name) {
	struct writer_object wo;

	init_writer_object(&wo, vhd_name);
	// Open VHD image file error
	if (get_last_error(&wo) == OPEN_FILE_ERROR) {
		return -1;
	}

	// Invaild or broken VHD image file
	if (!vaild_vhd(&wo)) {
		release_writer_object(&wo);
		return -2;
	}

	// The VHD image is not fixed which is still not support
	if (!fixed_vhd(&wo)) {
		release_writer_object(&wo);
		return -3;
	}

	size_vhd(&wo);

	// Data file is invaild
	int64_t data_file_size = get_file_size_by_name(data_name);
	if (data_file_size == 0) {
		release_writer_object(&wo);
		return -4;
	}

	// LBA is out of range
	int64_t total_written_bytes = write_hvd_sector_from_data_file(&wo, lba, data_name);
	enum writer_error err = get_last_error(&wo);
	if (err == LBA_OUT_OF_RANGE) {
		release_writer_object(&wo);
		return -5;
	}
	// Open data file error
	else if (err == OPEN_FILE_ERROR) {
		release_writer_object(&wo);
		return -6;
	}

	release_writer_object(&wo);
	return total_written_bytes;
}